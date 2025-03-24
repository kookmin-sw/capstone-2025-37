using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RNGNeeds.Editor
{
    #pragma warning disable 0618
    internal class PropertyData
    {
        public IProbabilityListEditorActions ProbabilityListEditorInterface;
        public PLDrawerSettings DrawerSettings;
        public ReorderableList ReorderableList;
        public TestResults TestResults;
        public GenericMenu PaletteDropdownMenu;
        public GenericMenu SelectionMethodDropdownMenu;
        public Dictionary<int, ItemInfoCache> ItemInfoCache;
        public List<ItemPropertyCache> ItemPropertyCache;
        public Rect OptionsRect;
        public Rect SubOptionsRect;
        public Rect MenuArrowRect;
        
        public FloatLabelField FloatLabelField;
        
        public string PropertyPath;
        public string NameOfProperty;
        public float MaxProbability;
        public int ValuesChangedFor = -1;
        public int UnlockedItems;
        public int IndexOfUnremovableItem;
        public bool ValueIsGenericType;
        public bool ValueIsObjectReference;
        public bool ValueCannotBeObtained;
        public bool SetupPropertiesRequired;
        public bool ShouldHighlightListElements;
        public bool CanDisplayItemInfo;
        public bool IsInfluencedList;
        public bool IsArray;
        public bool IsArrayAndNotCollection;
        
        public string SelectionMethodName;
        public string SelectionMethodTooltip;

        // Serialized Properties
        public SerializedProperty p_ProbabilityListProperty;
        public SerializedProperty p_ProbabilityItems;
        public SerializedProperty p_ID;
        public SerializedProperty p_PickCountMin;
        public SerializedProperty p_PickCountMax;
        public SerializedProperty p_PickCountCurve;
        public SerializedProperty p_PreventRepeat;
        public SerializedProperty p_ShuffleIterations;
        public SerializedProperty p_LinkPickCounts;
        public SerializedProperty p_MaintainPickCountIfDisabled;
        public SerializedProperty p_Seed;
        public SerializedProperty p_KeepSeed;

        // Modifiers
        public List<Rect> ModifierRects;
        public List<Rect> ProbabilityRects;
        public ModifierState ModifierState;
        public ModifierType ModifierType;
        public bool DrawModifierRects;
        public int SelectedModifier;
        public int HoveredProbabilityRect;
        public int HoveredListElement;

        // Colors & Styles
        public List<Color> StripeColors;
        public List<Color> ProbabilityItemColors;
        public readonly GUIStyle stripeIndexStyle = new GUIStyle();
        public readonly GUIStyle stripeNameStyle = new GUIStyle();
        public readonly GUIStyle stripePercentageStyle = new GUIStyle();

        public float GetStripeHeight
        {
            get
            {
                switch (DrawerSettings.StripeHeight)
                {
                    case StripeHeight.Compact:
                        return PLDrawerTheme.CompactStripeHeight;
                    case StripeHeight.Short:
                        return PLDrawerTheme.ShortStripeHeight;
                    case StripeHeight.Tall:
                        return PLDrawerTheme.TallStripeHeight;
                    default:
                        return PLDrawerTheme.NormalStripeHeight;
                }
            }
        }
        
        internal void SetHotStyles()
        {
            stripeIndexStyle.fontSize = 12;
            stripeIndexStyle.alignment = DrawerSettings.StripeHeight == StripeHeight.Compact ? TextAnchor.MiddleLeft : DrawerSettings.StripeHeight == StripeHeight.Short ? DrawerSettings.ShowPercentage ? TextAnchor.UpperLeft : TextAnchor.MiddleLeft : TextAnchor.UpperCenter;
            stripeIndexStyle.contentOffset = DrawerSettings.StripeHeight == StripeHeight.Compact ? new Vector2(6f, 0f) : DrawerSettings.StripeHeight == StripeHeight.Short ? new Vector2(6f, 0f) : Vector2.zero;

            stripeNameStyle.fontSize = PLDrawerTheme.versionSpecificFontSize;
            stripeNameStyle.alignment = DrawerSettings.StripeHeight == StripeHeight.Short ? DrawerSettings.ShowPercentage ? TextAnchor.UpperCenter : TextAnchor.MiddleCenter : TextAnchor.MiddleCenter;
            stripeNameStyle.fontStyle = FontStyle.Bold;
            stripeNameStyle.contentOffset = Vector2.zero;
            
            stripePercentageStyle.fontSize = 12;
            stripePercentageStyle.alignment = DrawerSettings.StripeHeight == StripeHeight.Compact ? TextAnchor.MiddleRight : TextAnchor.LowerCenter;
            stripePercentageStyle.contentOffset = DrawerSettings.StripeHeight == StripeHeight.Compact ? new Vector2(-4f, 0f) : new Vector2(0f, 1f);
        }
        
        public float ElementHeightCallback(int index)
        {
            var add = 0f;
            if(ItemInfoCache.TryGetValue(index, out var infoCache)) if (infoCache.InfluenceProviderExpanded) add = 24f + infoCache.influenceInfoHeight;
            
            return ValueCannotBeObtained ? 0 : p_ProbabilityItems.arraySize > 0
                ? EditorGUI.GetPropertyHeight(
                      ItemPropertyCache.Count < p_ProbabilityItems.arraySize ? 
                          p_ProbabilityItems.GetArrayElementAtIndex(index).FindPropertyRelative("m_Value") : ItemPropertyCache[index].p_Value, true) + add + 1f : 0;
        }
    }
    #pragma warning restore 0618
}