using System;
using System.Collections.Generic;
using System.Linq;
using StarphaseTools.Core;
using UnityEngine;

namespace RNGNeeds
{
    /// <summary>
    /// Represents a list of probability items of type <typeparamref name="T"/>. The class provides 
    /// methods for managing the list of items and their associated probabilities, as well as methods for selecting 
    /// items based on their probability distribution. Selection methods account for individual item states, like 
    /// enabled/disabled and locked/unlocked, affecting the overall selection process.
    /// </summary>
    [Serializable]
    public class ProbabilityList<T> : IProbabilityList
    {
        [SerializeField] private string m_PLID;
        [SerializeField] private List<ProbabilityItem<T>> m_ProbabilityItems = new List<ProbabilityItem<T>>();
        [SerializeField] private PreventRepeatMethod m_PreventRepeat;
        [SerializeField] private int m_ShuffleIterations = 1;
        [SerializeField] private int m_PickCountMin = 1;
        [SerializeField] private int m_PickCountMax = 1;
        [SerializeField] private bool m_LinkPickCounts = true;
        [SerializeField] private bool m_MaintainPickCountIfDisabled;
        [SerializeField] private AnimationCurve m_PickCountCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private uint m_Seed;
        [SerializeField] private bool m_KeepSeed;
        [SerializeField] private PickHistory m_PickHistory = new PickHistory();
        [SerializeField] private int m_LastPickCount;
        [SerializeField] private string m_SelectionMethodID = RNGNeedsCore.DefaultSelectionMethodID;
        private ISelectionMethod m_SelectionMethod;
        private List<int> m_LastPickedIndices = new List<int>();
        private List<T> m_LastPickedValues = new List<T>();
        
        #region Internal
        
        #if UNITY_EDITOR
        void IProbabilityListEditorActions.AddDefaultItem() => AddItem(default, true);
        Type IProbabilityListEditorActions.ItemType => typeof(T);
        List<Vector2> IProbabilityListEditorActions.GetInfluencedProbabilitiesLimits => ProbabilityItems.GetInfluencedProbabilitiesLimits();
        ItemProviderType IProbabilityListEditorActions.GetItemProviderTypes(int index) => ProbabilityItems[index].GetProviderTypes();
        
        int IProbabilityListEditorActions.IndexOfUnremovableItem
        {
            get
            {
                var itemCount = ItemCount;
                for (var i = 0; i < itemCount; i++)
                {
                    var probabilityItem = ProbabilityItems[i];
                    if (probabilityItem.BaseProbability >= 1f && IsItemWithAllRemainingProbability(i)) return i;
                    if (probabilityItem.Locked == false && IsOnlyUnlockedItemWithProbability(i)) return i;
                }

                return -1;
            }
        }
        #endif
        
        IProbabilityItem IProbabilityList.Item(int index) => m_ProbabilityItems[index];
        internal List<ProbabilityItem<T>> ProbabilityItems => m_ProbabilityItems;
        internal IProbabilityInfluenceProvider GetInfluenceProvider(int index) => ProbabilityItems[index].InfluenceProvider;
        
        internal ISelectionMethod SelectionMethodInstance
        {
            get
            {
                if (m_SelectionMethod == null) m_SelectionMethod = RNGNeedsCore.GetSelectionMethod(SelectionMethodID);
                return m_SelectionMethod;
            }
        }

        private bool IsOnlyUnlockedItemWithProbability(int index)
        {
            return ItemCount > 1 && UnlockedItemsCount == 1 && ProbabilityItems[index].Locked == false && ProbabilityItems[index].BaseProbability > 0f;
        }
        
        private bool IsItemWithAllRemainingProbability(int index)
        {
            return ItemCount > 1 && ProbabilityItems[index].BaseProbability >= 1f;
        }
        
        private bool Select(int pickCount)
        {
            this.SelectItemsInternal(pickCount, SelectionMethodInstance, out var selection);
            if (selection.Length == 0)
            {
                LastPickCount = 0;
                selection.Dispose();
                return false;
            }
            
            LastPickCount = selection.Length;
            PickHistory.AddEntries(selection);
            selection.Dispose();
            return true;
        }

        #endregion
        
        #region Helpers
        
        public bool IsListInfluenced
        {
            get
            {
                foreach (var probabilityItem in ProbabilityItems) if (probabilityItem.IsInfluencedItem) return true;
                return false;
            }
        }
        
        public int UnlockedItemsCount => ProbabilityItems.Count(x => x.Locked == false);
        
        #endregion
        
        #region Selection Method

        /// <summary>
        /// Gets or sets the identifier of the current Selection Method. Setting this property changes the Selection Method used for this list.
        /// </summary>
        /// <remarks>
        /// Consult the manual for available selection methods. If you are extending, register your selection method first via <see cref="RNGNeedsCore.RegisterSelectionMethod"/>. 
        /// If the identifier is incorrect or the method isn't registered, it defaults to the default selection method.
        /// </remarks>
        public string SelectionMethodID
        {
            get => m_SelectionMethodID;
            set
            {
                m_SelectionMethodID = value;
                m_SelectionMethod = RNGNeedsCore.GetSelectionMethod(SelectionMethodID);
            }
        }

        #endregion
        
        #region Repeat Prevention
        
        /// <summary>
        /// Defines the method for preventing repeated item selections. Note that:
        /// <list type="bullet">
        /// <item>This may alter the probability distribution results by reducing the chance of selecting recently picked items.</item>
        /// <item>This setting is ignored if there's only one enabled item in list.</item>
        /// </list>
        /// </summary>
        public PreventRepeatMethod PreventRepeat
        {
            get => m_PreventRepeat;
            set => m_PreventRepeat = value;
        }
        
        /// <summary>
        /// Specifies the number of shuffle iterations used by the <see cref="PreventRepeatMethod.Shuffle"/> to reduce repeat selections.
        /// </summary>
        /// <remarks>
        /// The value is always constrained between 1 and 5. More iterations enhance the prevention of repeats.
        /// </remarks>
        public int ShuffleIterations
        {
            get => m_ShuffleIterations;
            set => m_ShuffleIterations = Mathf.Clamp(value, 1, 5);
        }
        
        #endregion
        
        #region Pick Counts
        
        /// <summary>
        /// Specifies the minimum number of items to be picked using <see cref="PickValues()"/>.
        /// </summary>
        /// <remarks>Set <see cref="LinkPickCounts"/> to true to disable variable pick count.</remarks>
        public int PickCountMin
        {
            get => m_PickCountMin;
            set => m_PickCountMin = value;
        }

        /// <summary>
        /// Specifies the maximum number of items to be picked using <see cref="PickValues()"/>.
        /// </summary>
        /// <remarks>If <see cref="LinkPickCounts"/> is true, this property is ignored and the number of picks equals <see cref="PickCountMin"/>.</remarks>
        public int PickCountMax
        {
            get => LinkPickCounts ? PickCountMin : m_PickCountMax;
            set => m_PickCountMax = value;
        }
        
        /// <summary>
        /// If true, the number of items picked by <see cref="PickValues()"/> equals <see cref="PickCountMin"/>. Default is true.
        /// </summary>
        public bool LinkPickCounts
        {
            get => m_LinkPickCounts;
            set => m_LinkPickCounts = value;
        }
        
        /// <summary>
        /// If true, the selection process continues until the desired pick count is reached, even if the list contains disabled items. This may alter the probability distribution. Default is false.
        /// </summary>
        public bool MaintainPickCountIfDisabled
        {
            get => m_MaintainPickCountIfDisabled;
            set => m_MaintainPickCountIfDisabled = value;
        }
        
        /// <summary>
        /// Use this curve to adjust the bias when picking a number of items within the range of <see cref="PickCountMin"/> to <see cref="PickCountMax"/>.
        /// </summary>
        /// <remarks>The default curve is linear, resulting in an even random distribution.</remarks>
        public AnimationCurve PickCountCurve
        {
            get => m_PickCountCurve;
            set => m_PickCountCurve = value;
        }
        
        #endregion

        #region Seeding
        
        /// <summary>
        /// Gets the current seed of this list.
        /// </summary>
        /// <remarks>The seed is regenerated before each pick by default, unless <see cref="KeepSeed"/> is set to true.</remarks>
        public uint CurrentSeed => m_Seed;
        
        /// <summary>
        /// Generates and returns a new seed unless <see cref="KeepSeed"/> is true. This method is called automatically before each pick.
        /// If <see cref="KeepSeed"/> is True, you can use this property to set a custom seed.
        /// </summary>
        public uint Seed
        {
            get
            {
                if (KeepSeed == false || m_Seed < 1) m_Seed = RNGNeedsCore.NewSeed;
                return m_Seed;
            }
            set => m_Seed = value;
        }
        
        /// <summary>
        /// Specifies whether this list should retain its seed between picks.
        /// </summary>
        /// <remarks>
        /// Defaults to false, meaning a new seed is generated before each pick.
        /// </remarks>
        public bool KeepSeed
        {
            get => m_KeepSeed;
            set => m_KeepSeed = value;
        }
        
        #endregion

        #region History

        /// <summary>
        /// The history of recent item picks. It contains a list of <see cref="HistoryEntry"/> objects, which store the Index and Time of each pick. 
        /// </summary>
        /// <remarks>Since the history tracks picks by indices, the history will be cleared when items are removed from list.</remarks>
        public PickHistory PickHistory => m_PickHistory;

        /// <summary>
        /// Number of items selected by the last pick.
        /// </summary>
        public int LastPickCount
        {
            get => m_LastPickCount;
            internal set => m_LastPickCount = value;
        }
        
        /// <summary>
        /// Returns the index of the last picked item from the history, or -1 if the history is empty.
        /// </summary>
        public int LastPickedIndex => PickHistory.LatestIndex;

        /// <summary>
        /// Returns a shared list of indices from the last pick operation.
        /// </summary>
        /// <remarks>
        /// This property repopulates and returns a reference to a private list containing the indices from the last pick operation. It avoids creating a new list each time it is accessed.
        /// Alternatively, you can use your own list to retrieve history via <see cref="GetLastPickedIndices"/>
        /// </remarks>
        public List<int> LastPickedIndices
        {
            get
            {
                m_LastPickedIndices.Clear();
                PickHistory.GetLatestPicks(m_LastPickedIndices, LastPickCount);
                return m_LastPickedIndices;
            }
        }

        /// <summary>
        /// Adds indices from the last pick operation to the provided list.
        /// </summary>
        /// <param name="listToFill">The list that the indices will be added to.</param>
        public void GetLastPickedIndices(List<int> listToFill)
        {
            listToFill.AddRange(LastPickedIndices);
        }
        
        /// <summary>
        /// Returns the last picked value.
        /// </summary>
        /// <remarks>
        /// Returns 'default' value if history is empty.
        /// </remarks>
        public T LastPickedValue
        {
            get
            {
                var lastPickedIndex = LastPickedIndex;
                return lastPickedIndex < 0 ? default : ProbabilityItems[lastPickedIndex].Value;
            }
        }

        /// <summary>
        /// Returns a shared list of last picked values.
        /// </summary>
        /// <remarks>
        /// This property repopulates and returns a reference to a private list containing the values from the last pick operation. It avoids creating a new list each time it is accessed.
        /// Alternatively, you can use your own list to retrieve history via <see cref="GetLastPickedValues"/>.
        /// </remarks>
        public List<T> LastPickedValues
        {
            get
            {
                m_LastPickedValues.Clear();
                var lastPickedIndices = LastPickedIndices;
                var items = ProbabilityItems;
                foreach (var index in lastPickedIndices) m_LastPickedValues.Add(items[index].Value);
                return m_LastPickedValues;
            }
        }

        /// <summary>
        /// Adds last picked values to a provided list.
        /// </summary>
        /// <param name="listToFill">The list to which the values will be added.</param>
        public void GetLastPickedValues(List<T> listToFill)
        {
            listToFill.AddRange(LastPickedValues);
        }

        /// <summary>
        /// Sets the capacity of the <see cref="PickHistory"/> list.
        /// </summary>
        /// <param name="capacity">The desired size of the history. The minimum value is 1.</param>
        /// <remarks>
        /// The history list keeps recent picks at the 'top' of the list - the most recent pick being at index 0.
        /// Entries exceeding the specified capacity are removed from the list.
        /// </remarks>
        public void SetHistoryCapacity(int capacity) => PickHistory.Capacity = capacity;

        /// <summary>
        /// Clears the history of recent picks.
        /// </summary>
        public void ClearHistory()
        {
            PickHistory.ClearHistory();
            LastPickCount = 0;
        }

        #endregion

        #region Probability Operations

        /// <summary>
        /// Retrieves the calculated probability of an item.
        /// </summary>
        /// <param name="index">Index of the item in the list.</param>
        /// <returns>The calculated probability, including applied modifiers such as influence.</returns>
        /// <remarks>This method returns the probability that is evaluated during the selection process. To get the base probability of an item, use <see cref="GetItemBaseProbability"/>.</remarks>
        public float GetItemProbability(int index)
        {
            return ProbabilityItems[index].Probability;
        }

        /// <summary>
        /// Retrieves the base probability of an item.
        /// </summary>
        /// <param name="index">Index of the item in the list.</param>
        /// <returns>The unmodified base probability of the item.</returns>
        /// <remarks>This method returns the initial probability value of an item. To get the probability used in the selection process, use <see cref="GetItemProbability"/>.</remarks>
        public float GetItemBaseProbability(int index)
        {
            return ProbabilityItems[index].BaseProbability;
        }
        
        /// <summary>
        /// Adjusts the base probability of an item.
        /// </summary>
        /// <param name="index">Index of the item in the list.</param>
        /// <param name="amount">Amount to be added to the base probability.</param>
        /// <remarks>The final probability is clamped between 0 and 1.</remarks>
        public void AdjustItemBaseProbability(int index, float amount)
        {
            ProbabilityItems[index].BaseProbability += amount;
        }
        
        /// <summary>
        /// Sets the base probability of a specified item. If the "normalize" parameter is set to True, the following rules apply:
        /// <list type="bullet">
        /// <item>The probabilities of other items will be proportionally adjusted to maintain their current ratio.</item>
        /// <item>The probabilities of locked items will remain unchanged.</item>
        /// </list>
        /// </summary>
        /// <param name="index">The index of the item for which the base probability should be set.</param>
        /// <param name="baseProbability">The new probability for the item, a float value between 0 and 1. The provided value will be clamped within these bounds.</param>
        /// <param name="normalize">Determines whether the list should be normalized after the change. Default is True.</param>
        /// <returns>
        /// Returns True if the item was found and the probability was successfully set.
        /// Returns False if the item is locked or it is the only unlocked item in the list.
        /// </returns>
        /// <remarks>
        /// This method will log warnings if the index is out of range, the item at the provided index is locked, 
        /// or if the operation fails because the item is the only one in the list or the only unlocked item in the list.
        /// </remarks>
        public bool SetItemBaseProbability(int index, float baseProbability, bool normalize = true)
        {
            var itemCount = ProbabilityItems.Count;
            if (index < 0 || index >= itemCount)
            {
                RLogger.Log($"Index {index} out of range. There are {itemCount} items in this list. No item was changed.", LogMessageType.Warning);
                return false;
            }

            if (ProbabilityItems[index].Locked)
            {
                RLogger.Log($"Item at index {index} is Locked. Locked items cannot change probability.", LogMessageType.Hint);
                return false;
            }
            
            baseProbability = Mathf.Clamp(baseProbability, 0, 1);
            
            if (normalize == false)
            {
                ProbabilityItems[index].BaseProbability = baseProbability;
                return true;
            }
            
            if (itemCount < 2)
            {
                RLogger.Log($"Could not set Item {index} probability to {baseProbability:P5} because it is the only item in list. Total probability of list has to be 100%.", LogMessageType.Hint);
                return false;
            }

            if (ProbabilityItems.Count(item => !item.Locked) == 1)
            {
                RLogger.Log($"Could not set Item {index} probability to {baseProbability:P5} because it is the only unlocked item in list.", LogMessageType.Hint);
                return false;
            }

            var totalUnlockedProbability = 0f;
            var availableProbability = 0f;
            for (var i = 0; i < itemCount; i++)
            {
                var probabilityItem = ProbabilityItems[i];
                if (probabilityItem.Locked) continue;
                totalUnlockedProbability += probabilityItem.BaseProbability;
                if(i == index) continue;
                availableProbability += probabilityItem.BaseProbability;
            }

            baseProbability = Mathf.Clamp(baseProbability, 0f, totalUnlockedProbability);
            
            var oldProbability = ProbabilityItems[index].BaseProbability;
            ProbabilityItems[index].BaseProbability = baseProbability;
            var deltaProbability = baseProbability - oldProbability;
            
            if (availableProbability == 0)
            {
                RLogger.Log($"Could not set Item {index} probability to {baseProbability:P5} because there is only {availableProbability:P5} available probability in list. Some probability might be unavailable due to locked items or items with 0% probability.", LogMessageType.Hint);
                ProbabilityItems[index].BaseProbability = oldProbability;
                return false;
            }

            for (var i = 0; i < itemCount; i++)
            {
                if(i == index) continue;
                var probabilityItem = ProbabilityItems[i];
                if(probabilityItem.Locked) continue;
                probabilityItem.BaseProbability -= (probabilityItem.BaseProbability / availableProbability) * deltaProbability;
                if (probabilityItem.BaseProbability < .0000001f) probabilityItem.BaseProbability = 0f;
            }

            NormalizeProbabilities();
            return true;
        }

        /// <summary>
        /// Resets all probabilities in the list to be equal.
        /// </summary>
        /// <remarks>Locked items are not affected by this operation.</remarks>
        public void ResetAllProbabilities()
        {
            if (ItemCount < 1) return;
            var unlockedCount = 0;
            var unlockedBaseProbability = 0f;
            
            foreach (var probabilityItem in ProbabilityItems)
            {
                if (probabilityItem.Locked) continue;
                unlockedCount++;
                unlockedBaseProbability += probabilityItem.BaseProbability;
            }
            
            foreach (var probabilityItem in ProbabilityItems)
            {
                if(probabilityItem.Locked) continue;
                probabilityItem.BaseProbability = unlockedBaseProbability / unlockedCount;
            }
        }
        
        /// <summary>
        /// Recalculates base probabilities of items to ensure the total probability of the list equals to 1.
        /// Locked items are not affected.
        /// </summary>
        /// <returns>The new total probability of the list.</returns>
        /// <remarks>This operation is performed automatically when adding or removing items, or when setting an item's base probability via <see cref="SetItemBaseProbability"/> or <see cref="SetItemProperties"/>.</remarks>
        public float NormalizeProbabilities()
        {
            var totalBaseProbability = ProbabilityItems.Sum(x => x.BaseProbability);
            var unlockedProbability = ProbabilityItems.Where(x => !x.Locked).Sum(x => x.BaseProbability);
            if (ProbabilityItems.Count(x => x.Locked == false) == 0) return totalBaseProbability;
            var scalingFactor = (1.0f - (totalBaseProbability - unlockedProbability)) / unlockedProbability;

            var newTotal = 0f;
            foreach (var item in ProbabilityItems)
            {
                if (item.Locked == false)
                {
                    item.BaseProbability *= scalingFactor;
                    if (float.IsNaN(item.BaseProbability) || item.BaseProbability < .0000001f) item.BaseProbability = 0f;
                }

                newTotal += item.BaseProbability;
            }
            
            return newTotal;
        }

        #endregion
        
        #region Item Operations

        /// <summary>
        /// Number of items in list.
        /// </summary>
        public int ItemCount => m_ProbabilityItems.Count;

        /// <summary>
        /// Creates a new <see cref="ProbabilityItem{T}"/> for the provided value and adds it to the list. The new item's probability is automatically calculated based on the other items' probabilities. The following rules apply:
        /// <list type="bullet">
        /// <item>The probabilities of other items are reduced proportionally to maintain their current ratio.</item>
        /// <item>The probabilities of locked items remain unchanged.</item>
        /// <item>If all items in the list are locked, the new item is added with a probability of 0.</item>
        /// </list>
        /// </summary>
        /// <returns>The newly created <see cref="ProbabilityItem{T}"/></returns>
        /// <param name="value">The value to be added.</param>
        /// <param name="enabled">Specifies whether the added item should be enabled in the list. The default is True.</param>
        /// <param name="locked">Specifies whether the added item should be locked in the list. The default is False.</param>
        public ProbabilityItem<T> AddItem(T value, bool enabled = true, bool locked = false)
        {
            if (ProbabilityItems.Count == 0)
            {
                return AddItem(value, 1f, enabled, locked);
            }
            
            var totalUnlockedBaseProbability = 0f;
            var unlockedCount = 0;
            var totalBaseProbability = 0f;

            foreach (var pItem in ProbabilityItems)
            {
                totalBaseProbability += pItem.BaseProbability;
                if (pItem.Locked) continue;
                totalUnlockedBaseProbability += pItem.BaseProbability;
                if(pItem.BaseProbability > 0f) unlockedCount++;
            }

            var resultingProbability = 0f;
            
            // if (Mathf.Approximately(totalBaseProbability, 1f) == false) resultingProbability = 1f - totalBaseProbability;
            if (Mathf.Abs(totalBaseProbability - 1f) > 0.0001f) 
            {
                resultingProbability = 1f - totalBaseProbability;
            }
            else
            if (unlockedCount > 0)
            {
                var decreaseFactor = 1f / (unlockedCount + 1);
                
                resultingProbability = totalUnlockedBaseProbability / (unlockedCount + 1);
                foreach (var pItem in ProbabilityItems) if (pItem.Locked == false) pItem.BaseProbability *= (1f - decreaseFactor);
            }
            
            return AddItem(value, resultingProbability, enabled, locked);
        }
        
        /// <summary>
        /// Creates a new <see cref="ProbabilityItem{T}"/> for the provided value and adds it to the list.
        /// </summary>
        /// <returns>The newly created <see cref="ProbabilityItem{T}"/></returns>
        /// <param name="value">The value to be added.</param>
        /// <param name="baseProbability">The desired probability for the item. The provided value will be clamped between 0 and 1.</param>
        /// <param name="enabled">Specifies whether the added item should be enabled in the list. The default is True.</param>
        /// <param name="locked">Specifies whether the added item should be locked in the list. The default is False.</param>
        /// <remarks>
        /// Remember to call <see cref="NormalizeProbabilities"/> after you are done adding items to ensure the total probability of the list is equal to 1.
        /// </remarks>
        public ProbabilityItem<T> AddItem(T value, float baseProbability, bool enabled = true, bool locked = false)
        {
            var newItem = new ProbabilityItem<T>(value, baseProbability, enabled, locked); 
            AddItem(newItem);
            return newItem;
        }
        
        /// <summary>
        /// Adds the provided <see cref="ProbabilityItem{T}"/> to the list.
        /// </summary>
        /// <param name="item">The ProbabilityItem to be added.</param>
        /// <remarks>
        /// The item is added 'as is'. Set its properties such as Probability, Enabled, or Locked beforehand, or use <see cref="SetItemProperties"/> after adding.
        /// Remember to call <see cref="NormalizeProbabilities"/> after you are done adding items to ensure the total probability of the list is equal to 1.
        /// </remarks>
        public void AddItem(ProbabilityItem<T> item)
        {
            ProbabilityItems.Add(item);
        }

        /// <summary>
        /// Removes the provided <see cref="ProbabilityItem{T}"/> from the list.
        /// </summary>
        /// <param name="probabilityItem">The ProbabilityItem to remove.</param>
        /// <param name="normalize">Specifies whether the probabilities in the list should be normalized after removal. The default is True.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        /// <remarks>
        /// The pick history will be cleared after the item is removed.
        /// Use this method overload if you have a reference to the ProbabilityItem obtained via <see cref="GetProbabilityItem"/> or <see cref="TryGetProbabilityItem"/>.
        /// </remarks>
        public bool RemoveItem(ProbabilityItem<T> probabilityItem, bool normalize = true)
        {
            return ProbabilityItems.Contains(probabilityItem) && RemoveItemAtIndex(ProbabilityItems.IndexOf(probabilityItem), normalize);
        }

        /// <summary>
        /// Removes an item with the provided value from the list.
        /// </summary>
        /// <param name="value">The value you wish to remove.</param>
        /// <param name="normalize">Specifies whether the probabilities in the list should be normalized after removal. The default is True.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        /// <remarks>
        /// The pick history will be cleared after the item is removed.
        /// This overload only works with reference types. Value types should use <see cref="RemoveItemAtIndex"/>.
        /// </remarks>
        public bool RemoveItem(T value, bool normalize = true)
        {
            for (var i = 0; i < ProbabilityItems.Count; i++)
            {
                var probabilityItem = ProbabilityItems[i];
                if (Equals(probabilityItem.Value, value) == false) continue;
                return RemoveItemAtIndex(i, normalize);
            }

            return false;
        }

        /// <summary>
        /// Removes the <see cref="ProbabilityItem{T}"/> at the specified index from the list.
        /// </summary>
        /// <param name="index">The index of the item you wish to remove.</param>
        /// <param name="normalize">Specifies whether the probabilities in the list should be normalized after removal. The default is True.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        /// <remarks>
        /// The pick history will be cleared after the item is removed.
        /// </remarks>
        public bool RemoveItemAtIndex(int index, bool normalize = true)
        {
            if (index < 0 || index >= ProbabilityItems.Count) return false;
            
            if (normalize)
            {
                if(IsOnlyUnlockedItemWithProbability(index))
                {
                    RLogger.Log("Cannot remove the only unlocked item if it's probability is greater than zero. Other locked items cannot change probability, so the list cannot be normalized to sum up to 1 probability. Unlock at least one other item before removing this one or use parameter 'normalize = false' to remove item regardless of the resulting probability.", LogMessageType.Hint);
                    return false;
                }

                if (IsItemWithAllRemainingProbability(index))
                {
                    RLogger.Log("Cannot remove item with 100% probability if there are other items in list. Items with 0% probability cannot be increased and normalized. Use parameter 'normalize = false' to remove item regardless of the resulting probability.", LogMessageType.Hint);
                    return false;
                }
            }
            
            ProbabilityItems.RemoveAt(index);
            if (normalize) NormalizeProbabilities();
            ClearHistory();
            return true;
        }
        
        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        /// <remarks>
        /// This operation also clears the pick history.
        /// </remarks>
        public void ClearList()
        {
            ProbabilityItems.Clear();
            ClearHistory();
        }

        /// <summary>
        /// Finds the index of the provided ProbabilityItem.
        /// </summary>
        /// <param name="probabilityItem">The <see cref="ProbabilityItem{T}"/> to find.</param>
        /// <returns>The index of the item if found, -1 otherwise.</returns>
        public int IndexOf(ProbabilityItem<T> probabilityItem)
        {
            return ProbabilityItems.IndexOf(probabilityItem);
        }

        /// <summary>
        /// Retrieves a ProbabilityItem at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The <see cref="ProbabilityItem{T}"/> found at the specified index, or null if the index is out of range.</returns>
        public ProbabilityItem<T> GetProbabilityItem(int index)
        {
            if (index < 0 || index >= ProbabilityItems.Count) return null;
            return ProbabilityItems[index];
        }

        /// <summary>
        /// Attempts to retrieve a ProbabilityItem at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <param name="probabilityItem">The retrieved <see cref="ProbabilityItem{T}"/>, or null if the index is out of range.</param>
        /// <returns>True if the item was successfully retrieved, false otherwise.</returns>
        public bool TryGetProbabilityItem(int index, out ProbabilityItem<T> probabilityItem)
        {
            probabilityItem = null;
            if (index < 0 || index >= ProbabilityItems.Count) return false;
            probabilityItem = ProbabilityItems[index];
            return true;
        }

        /// <summary>
        /// Sets an influence provider for the ProbabilityItem at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to modify.</param>
        /// <param name="influenceProvider">The influence provider to set.</param>
        /// <remarks>
        /// <para>An influence provider provides a way to dynamically influence the probability of an item being selected.</para>
        /// </remarks>
        public void SetItemInfluenceProvider(int index, IProbabilityInfluenceProvider influenceProvider)
        {
            if (TryGetProbabilityItem(index, out var probabilityItem)) probabilityItem.InfluenceProvider = influenceProvider;
        }

        /// <summary>
        /// Sets the properties for the ProbabilityItem at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to modify.</param>
        /// <param name="baseProbability">The desired probability for the item. The value will be clamped between 0 and 1.</param>
        /// <param name="enabled">Whether the item should be enabled in the list.</param>
        /// <param name="locked">Whether the item should be locked in the list.</param>
        /// <returns>True if the properties were successfully set, false otherwise.</returns>
        /// <remarks>
        /// <para>If any of the property changes fail, all changes are rolled back to their previous state.</para>
        /// <para>Remember to call <see cref="NormalizeProbabilities"/> after you are done setting properties to ensure the total probability of the list equals 1.</para>
        /// </remarks>
        public bool SetItemProperties(int index, float baseProbability, bool enabled, bool locked)
        {
            if (index < 0 || index >= ProbabilityItems.Count) return false;
            var probabilityItem = ProbabilityItems[index];
            var enabledState = probabilityItem.Enabled;
            var lockedState = probabilityItem.Locked;
            var probabilityState = probabilityItem.BaseProbability;
            if (SetItemBaseProbability(index, baseProbability) && SetItemEnabled(index, enabled) && SetItemLocked(index, locked)) return true;
            
            // Rollback if something failed
            probabilityItem.Enabled = enabledState;
            probabilityItem.Locked = lockedState;
            probabilityItem.BaseProbability = probabilityState;
            return false;
        }
        
        /// <summary>
        /// Sets the Enabled status of a <see cref="ProbabilityItem{T}"/> at the given index.
        /// </summary>
        /// <param name="index">The index of the item to modify.</param>
        /// <param name="enabled">If set to <c>true</c>, the item will be eligible for selection. If <c>false</c>, the item will be ignored during selection.</param>
        /// <returns><c>true</c> if the operation was successful, <c>false</c> if the index is out of range.</returns>
        /// <remarks>
        /// <para>Items that are not enabled are ignored during the selection process, regardless of their probability.</para>
        /// </remarks>
        public bool SetItemEnabled(int index, bool enabled)
        {
            if (index < 0 || index >= ProbabilityItems.Count) return false;
            ProbabilityItems[index].Enabled = enabled;
            return true;
        }
        
        /// <summary>
        /// Sets the enabled state for all items in the Probability List.
        /// </summary>
        /// <param name="enabled">A boolean indicating whether to enable or disable all items.</param>
        /// <remarks>
        /// This method allows you to easily enable or disable all items within the Probability List.
        /// When setting items to be enabled, they will participate in the selection process; when disabled,
        /// they will be excluded from selection.
        /// </remarks>
        public void SetAllItemsEnabled(bool enabled)
        {
            foreach (var probabilityItem in ProbabilityItems)
            {
                probabilityItem.Enabled = enabled;
            }
        }

        /// <summary>
        /// Sets the Locked status of a <see cref="ProbabilityItem{T}"/> at the given index.
        /// </summary>
        /// <param name="index">The index of the item to modify.</param>
        /// <param name="locked">If set to <c>true</c>, the item's probability will not be affected by other operations (e.g., adding or removing items, normalizing probabilities). If <c>false</c>, the item's probability may be adjusted by these operations.</param>
        /// <returns><c>true</c> if the operation was successful, <c>false</c> if the index is out of range.</returns>
        /// <remarks>
        /// <para>Locking an item can be useful to ensure that certain items retain a fixed probability, regardless of changes to other items in the list.</para>
        /// </remarks>
        public bool SetItemLocked(int index, bool locked)
        {
            if (index < 0 || index >= ProbabilityItems.Count) return false;
            ProbabilityItems[index].Locked = locked;
            return true;
        }
        
        /// <summary>
        /// Sets the locked state for all items in the Probability List.
        /// </summary>
        /// <param name="locked">A boolean indicating whether to lock or unlock all items.</param>
        /// <remarks>
        /// This method allows you to easily lock or unlock all items within the Probability List.
        /// When an item is locked, its probability cannot be changed, ensuring it remains constant
        /// even during normalization or other operations that may affect probabilities.
        /// </remarks>
        public void SetAllItemsLocked(bool locked)
        {
            foreach (var probabilityItem in ProbabilityItems)
            {
                probabilityItem.Locked = locked;
            }
        }

        #endregion
        
        #region Value Picks
        /// <summary>
        /// Picks a single random value from the list according to probability distribution.
        /// </summary>
        /// <returns>The randomly picked value, or default value of type T if the selection was unsuccessful.</returns>
        /// <remarks>
        /// For value types, a default value is returned even if the selection was unsuccessful.
        /// For example, if T is int or string, and no value is selected, the result will be 0 or an empty string, respectively.
        /// If you need to determine the success of the selection, use <see cref="TryPickValue"/> instead.
        /// </remarks>
        public T PickValue()
        {
            _ = Seed;
            return Select(1) ? LastPickedValue : default;
        }
        
        /// <summary>
        /// Attempts to pick a single random value from the list according to probability distribution.
        /// </summary>
        /// <param name="value">Output parameter to store the selected value, or the default value of type T if the selection was unsuccessful.</param>
        /// <returns>True if the selection was successful, false otherwise.</returns>
        public bool TryPickValue(out T value)
        {
            _ = Seed;
            var success = Select(1);
            if (success)
            {
                value = LastPickedValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Selects multiple random values from the list according to the defined probability distribution. 
        /// The number of selected values is within the range defined by <see cref="PickCountMin"/> and <see cref="PickCountMax"/>.
        /// </summary>
        /// <returns>A shared list of selected values. If the selection was unsuccessful, the list will be empty.</returns>
        /// <remarks>When there are disabled items in the list, the actual number of successful selections might be lower than the desired pick count. This is because the method only returns successfully selected values, and disabled values are ignored during the selection process.</remarks>
        public List<T> PickValues()
        {
            _ = Select(PickCountCurve.PickRandomInt(PickCountMin, PickCountMax, Seed));
            return LastPickedValues;
            // return PickValues(PickCountMin, PickCountMax);
        }

        /// <summary>
        /// Selects a specified number of random values from the list according to the defined probability distribution.
        /// </summary>
        /// <param name="pickCountMin">Minimum number of values to select.</param>
        /// <param name="pickCountMax">Maximum number of values to select.</param>
        /// <returns>A shared list of selected values. If the selection was unsuccessful, the list will be empty.</returns>
        public List<T> PickValues(int pickCountMin, int pickCountMax)
        {
            _ = Select(PickCountCurve.PickRandomInt(pickCountMin, pickCountMax, Seed));
            return LastPickedValues;
            // return PickValues(PickCountCurve.PickRandomInt(pickCountMin, pickCountMax, CurrentSeed));
        }

        /// <summary>
        /// Selects a specific number of random values from the list according to the defined probability distribution.
        /// </summary>
        /// <param name="pickCount">The number of values to select.</param>
        /// <returns>A shared list of selected values. If the selection was unsuccessful, the list will be empty.</returns>
        public List<T> PickValues(int pickCount)
        {
            _ = Seed;
            _ = Select(pickCount);
            return LastPickedValues;
        }

        /// <summary>
        /// Adds random values to the provided list according to the defined probability distribution.
        /// </summary>
        /// <param name="listToFill">The list to which the selected values will be added.</param>
        /// <returns>True if at least one value was successfully added to the list, false otherwise.</returns>
        /// <remarks>Number of picked values will be in range of <see cref="PickCountMin"/> and <see cref="PickCountMax"/></remarks>
        public bool PickValues(List<T> listToFill)
        {
            var success = Select(PickCountCurve.PickRandomInt(PickCountMin, PickCountMax, Seed));
            if (success == false) return false;
            listToFill.AddRange(LastPickedValues);
            return true;
            // return PickValues(listToFill, PickCountMin, PickCountMax);
        }

        /// <summary>
        /// Adds a specified number of random values to the provided list according to the defined probability distribution.
        /// </summary>
        /// <param name="listToFill">The list to which the selected values will be added.</param>
        /// <param name="pickCountMin">Minimum number of values to select.</param>
        /// <param name="pickCountMax">Maximum number of values to select.</param>
        /// <returns>True if at least one value was successfully added to the list, false otherwise.</returns>
        public bool PickValues(List<T> listToFill, int pickCountMin, int pickCountMax)
        {
            var success = Select(PickCountCurve.PickRandomInt(pickCountMin, pickCountMax, Seed));
            if (success == false) return false;
            listToFill.AddRange(LastPickedValues);
            return true;
            // return PickValues(listToFill, PickCountCurve.PickRandomInt(pickCountMin, pickCountMax, CurrentSeed));
        }

        /// <summary>
        /// Adds a specific number of random values to the provided list according to the defined probability distribution.
        /// </summary>
        /// <param name="listToFill">The list to which the selected values will be added.</param>
        /// <param name="pickCount">The number of values to select.</param>
        /// <returns>True if at least one value was successfully added to the list, false otherwise.</returns>
        public bool PickValues(List<T> listToFill, int pickCount)
        {
            _ = Seed;
            var success = Select(pickCount);
            if (success == false) return false;
            listToFill.AddRange(LastPickedValues);
            return true;
        }

        /// <summary>
        /// Picks a single random value from the list according to probability distribution and also provides the index of the selected value.
        /// </summary>
        /// <returns>A tuple containing the selected value and its index, or the default value of T and -1 as the index if the selection was unsuccessful.</returns>
        /// <remarks>
        /// For value types, a default value is returned even if the selection was unsuccessful.
        /// For example, if T is int or string, and no value is selected, the result will be 0 or an empty string, respectively, with an index of -1.
        /// If you need to determine the success of the selection, use <see cref="TryPickValueWithIndex"/> instead.
        /// </remarks>
        public (T Value, int Index) PickValueWithIndex()
        {
            return TryPickValueWithIndex(out var value, out var index) ? (value, index) : (default, -1);
        }
        
        /// <summary>
        /// Attempts to pick a random value from the list according to probability distribution, along with its index.
        /// If the pick is successful, the method returns true, the picked value and its index are stored in the output parameters.
        /// If the pick is not successful, the method returns false, and the output parameters are set to default values.
        /// </summary>
        /// <param name="Value">The output parameter to store the picked value, or the default value of type T if the pick is not successful.</param>
        /// <param name="Index">The output parameter to store the index of the picked value, or -1 if the pick is not successful.</param>
        /// <returns>True if the pick is successful, false otherwise.</returns>
        public bool TryPickValueWithIndex(out T Value, out int Index)
        {
            _ = Seed;
            var success = Select(1);
            if (success)
            {
                Value = LastPickedValue;
                Index = LastPickedIndex;
                return true;
            }

            Value = default;
            Index = -1;
            return false;
        }

        public TestResults RunTest()
        {
            return this.Test();
        }
        
        #endregion
    }
}