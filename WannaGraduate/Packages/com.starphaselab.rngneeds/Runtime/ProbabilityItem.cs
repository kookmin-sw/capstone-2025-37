using System;
using System.Linq;
using StarphaseTools.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RNGNeeds
{
    /// <summary>
    /// This class represents an item in the <see cref="ProbabilityList{T}"/>, carrying the value of the item and its associated probabilities.
    /// </summary>
    [Serializable]
    public class ProbabilityItem<T> : IProbabilityItem
    {
        [SerializeField] private T m_Value;
        [SerializeField] private float m_BaseProbability;
        [SerializeField] private bool m_Enabled;
        [SerializeField] private bool m_Locked;
        [SerializeField] private Vector2 m_InfluenceSpread = new Vector2(0f, 1f);
        [SerializeReference] private Object m_InfluenceProvider;
        
        private IProbabilityInfluenceProvider m_CachedInfluenceProvider;
        private bool m_ValueIsInfluenceProvider;
        private bool m_IsInfluencedItem;
        private bool m_IsValueType;
        private bool PropertiesUpdated { get; set; }
        private string cachedProvider;
        
        // public ProbabilityItem(T value = default, float baseProbability = 0f, bool enabled = true, bool locked = false)
        public ProbabilityItem(T value, float baseProbability, bool enabled = true, bool locked = false)
        {
            Value = value;
            BaseProbability = baseProbability;
            Enabled = enabled;
            Locked = locked;
            m_IsValueType = typeof(T).IsValueType;
        }

        #region Internal

        void IProbabilityItem.UpdateProperties()
        {
            m_ValueIsInfluenceProvider = m_IsValueType == false && GetProviderTypes().HasAny(ItemProviderType.InfluenceProvider);
            m_CachedInfluenceProvider = m_ValueIsInfluenceProvider
                ? m_Value as IProbabilityInfluenceProvider
                : m_InfluenceProvider as IProbabilityInfluenceProvider;

            cachedProvider = m_CachedInfluenceProvider?.ToString();
            m_IsInfluencedItem = m_CachedInfluenceProvider != null;
            PropertiesUpdated = true;
        }
        
        private void ClampSpreadToProbability()
        {
            var probability = BaseProbability;
            m_InfluenceSpread.x = Mathf.Clamp(m_InfluenceSpread.x, 0f, probability);
            m_InfluenceSpread.y = Mathf.Clamp(m_InfluenceSpread.y, probability, 1f);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value held by this ProbabilityItem.
        /// </summary>
        public T Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                ((IProbabilityItem)this).UpdateProperties();
            }
        }
        
        /// <summary>
        /// Gets or sets the base probability of the item, a value between 0 and 1. 
        /// If set to a value outside this range, it will be clamped. 
        /// This base probability can be influenced by modifiers but this property represents the unaltered probability.
        /// </summary>
        public float BaseProbability { get => m_BaseProbability;
            set
            {
                m_BaseProbability = Mathf.Clamp(value, 0f, 1f); 
                ClampSpreadToProbability();
            }
        }
        
        /// <summary>
        /// Gets the current probability of the item, taking into account any applied modifiers. 
        /// This is the actual probability used during the selection process. 
        /// If the item has no modifiers, it simply returns the base probability.
        /// </summary>
        public float Probability => IsInfluencedItem ? GetInfluencedProbability(InfluenceProvider.ProbabilityInfluence) : m_BaseProbability;

        /// <summary>
        /// Calculates and returns the influenced probability of the item being selected, based on the given influence value.
        /// If the item is not influenced, it simply returns the base probability.
        /// </summary>
        public float GetInfluencedProbability(float influence)
        {
            return IsInfluencedItem ? ProbabilityTools.CalculateInfluencedProbability(BaseProbability, InfluenceSpread.x, InfluenceSpread.y, influence) : m_BaseProbability ;
        }
        
        /// <summary>
        /// Gets or sets whether the item is enabled for selection. If set to false, the item is ignored during the selection process.
        /// </summary>
        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        /// <summary>
        /// Gets or sets whether the item's base probability is locked from being altered. If set to true, the base probability cannot be changed.
        /// </summary>
        public bool Locked
        {
            get => m_Locked;
            set => m_Locked = value;
        }

        #endregion
        
        #region Probability Influence Provider

        /// <summary>
        /// Gets or sets the degree to which influence can affect the probability of this item. The spread is specified as a Vector2, 
        /// where x represents the lowest possible probability and y represents the highest possible probability due to influence.
        /// </summary>
        public Vector2 InfluenceSpread
        {
            get => m_InfluenceSpread;
            set
            {
                m_InfluenceSpread = value;
                ClampSpreadToProbability();
            }
        }
        
        /// <summary>
        /// Gets or sets an external provider of influence for this item. Note that if the item's value itself is an influence provider, 
        /// it will be preferred over this external provider.
        /// </summary>
        public IProbabilityInfluenceProvider InfluenceProvider
        {
            get
            {
                if (IsInfluencedItem == false || m_CachedInfluenceProvider != null) return m_CachedInfluenceProvider;
                ((IProbabilityItem)this).UpdateProperties();
                return m_CachedInfluenceProvider;
            } 
            set
            {
                m_InfluenceProvider = value as Object;
                ((IProbabilityItem)this).UpdateProperties();
            }
        }
        
        /// <summary>
        /// Gets whether the item's value also serves as an influence provider. If the item's value type implements 
        /// the <see cref="IProbabilityInfluenceProvider"/> interface, it can provide its own influence, overriding any external provider. 
        /// </summary>
        public bool ValueIsInfluenceProvider
        {
            get
            {
                if(PropertiesUpdated == false) ((IProbabilityItem)this).UpdateProperties();
                return m_ValueIsInfluenceProvider;
            }
        }

        /// <summary>
        /// Gets whether this item is influenced by any source (either an external influence provider or the item's value itself, if it's an influence provider). 
        /// </summary>
        public bool IsInfluencedItem
        {
            get
            {
                if(PropertiesUpdated == false) ((IProbabilityItem)this).UpdateProperties();
                return m_IsInfluencedItem;
            }
        }
        
        #endregion

        /// <summary>
        /// Determines the types of interfaces implemented by the 'Value' held by this ProbabilityItem.
        /// It checks if the value implements any of the following interfaces: 
        /// <see cref="IProbabilityItemInfoProvider"/>, <see cref="IProbabilityInfluenceProvider"/>, or <see cref="IProbabilityItemColorProvider"/>.
        /// </summary>
        /// <returns>A flag enumeration <see cref="ItemProviderType"/> indicating which of the aforementioned interfaces are implemented by the 'Value'.</returns>
        public ItemProviderType GetProviderTypes()
        {
            var providerTypes = ItemProviderType.None;
            if (Value == null) return providerTypes;
            var interfaces = Value.GetType().GetInterfaces();
            if (interfaces.Contains(typeof(IProbabilityItemInfoProvider))) providerTypes |= ItemProviderType.InfoProvider;
            if (interfaces.Contains(typeof(IProbabilityInfluenceProvider))) providerTypes |= ItemProviderType.InfluenceProvider;
            if (interfaces.Contains(typeof(IProbabilityItemColorProvider))) providerTypes |= ItemProviderType.ColorProvider;
            return providerTypes;
        }
    }
}