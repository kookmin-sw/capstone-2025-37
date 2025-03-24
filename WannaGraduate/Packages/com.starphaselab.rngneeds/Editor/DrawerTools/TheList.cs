using StarphaseTools.Core;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class TheList
    {
        private static readonly GUIContent m_TempContent = new GUIContent();
        private const float borderWidthFactor = 1f;
        private const float borderWidthOffset = 2f;
        private const float previewRectHeight = 10f;
        public static bool ShouldColorizeBars { get; set; }
        public static bool DrawInfluenceProviderToggle { get; set; }
        
        internal static void ShiftDataOnReorder(PropertyData propertyData, int oldIndex, int newIndex)
        {
            var movedInfoCache = propertyData.ItemInfoCache[oldIndex];
            // var movedPropertyCache = propertyData.ItemPropertyCache[oldIndex];
            var movedTestResult = -1;

            var shiftTestResults = propertyData.TestResults.indexPicks.Count > 0;
            if (shiftTestResults) movedTestResult = propertyData.TestResults.indexPicks[oldIndex];

            var step = oldIndex < newIndex ? 1 : -1;
            var start = oldIndex + step;
            var end = newIndex + step;

            for (var i = start; i != end; i += step)
            {
                var targetIndex = i - step;

                propertyData.ItemInfoCache[i].index = targetIndex.ToString();
                propertyData.ItemInfoCache[targetIndex] = propertyData.ItemInfoCache[i];

                // propertyData.ItemPropertyCache[targetIndex] = propertyData.ItemPropertyCache[i];
                if (shiftTestResults) propertyData.TestResults.indexPicks[targetIndex] = propertyData.TestResults.indexPicks[i];
            }

            movedInfoCache.index = newIndex.ToString();
            propertyData.ItemInfoCache[newIndex] = movedInfoCache;
            // propertyData.ItemPropertyCache[newIndex] = movedPropertyCache;
            
            if (shiftTestResults) propertyData.TestResults.indexPicks[newIndex] = movedTestResult;
        }

        private static void DrawUtilitySlider(PropertyData propertyData, Rect position, int index, bool drawInfluenceSlider, bool normalizeBars, bool showSpreadOnBars)
        {
            var sliderRect = position;
        
            var itemPropertyCache = propertyData.ItemPropertyCache[index];
            var range = itemPropertyCache.p_InfluenceSpread.vector2Value;
            var probability = propertyData.GetBaseProbability(index);
            
            if (drawInfluenceSlider)
            {
                EditorGUI.MinMaxSlider(sliderRect, ref range.x, ref range.y, 0f, 1f);
                EditorGUI.DrawRect(new Rect(sliderRect.x + ((sliderRect.width - 12f) * probability) + 5f, position.y + 4f, 2f, 10f), Color.white);
            }
            else
            {
                var width = normalizeBars ? sliderRect.width * (probability / propertyData.MaxProbability) : sliderRect.width * probability;
                var height = showSpreadOnBars ? 6f : previewRectHeight;
                var posY = showSpreadOnBars ? 6f : 4f;
        
                var previewRect = new Rect(sliderRect.x, position.y + posY, width, height);
        
                if (!EditorGUIUtility.isProSkin && ShouldColorizeBars)
                {
                    var borderWidth = borderWidthFactor * EditorGUIUtility.pixelsPerPoint;
                    var previewBackgroundRect = new Rect(sliderRect.x - borderWidth, position.y + borderWidthOffset + borderWidth,
                        showSpreadOnBars ? (sliderRect.width * propertyData.ItemInfoCache[index].InfluencedProbabilityLimits.x) + borderWidth * 2f : 
                            width + borderWidth * 2f, 12f);
        
                    if (Event.current.type == EventType.Repaint)
                        PLDrawerTheme.RectBorder.Draw(previewBackgroundRect, false, false, false, false);
        
                    EditorGUI.DrawRect(previewRect, PLDrawerTheme.DimBackgroundColor);
                }
        
                if (showSpreadOnBars)
                {
                    var spreadBarsColor = EditorGUIUtility.isProSkin && ShouldColorizeBars ? propertyData.ProbabilityItemColors[index] : Color.gray;
                    var spreadMaxRect = new Rect(sliderRect.x, position.y + 4f, sliderRect.width * propertyData.ItemInfoCache[index].InfluencedProbabilityLimits.y, 2f);
                    var spreadMinRect = new Rect(sliderRect.x, position.y + 12f, sliderRect.width * propertyData.ItemInfoCache[index].InfluencedProbabilityLimits.x, 2f);
                    EditorGUI.DrawRect(spreadMaxRect, spreadBarsColor);
                    EditorGUI.DrawRect(spreadMinRect, spreadBarsColor);
                }
        
                if (propertyData.DrawerSettings.DimColors) EditorGUI.DrawRect(previewRect, PLDrawerTheme.DimBackgroundColor);
                EditorGUI.DrawRect(previewRect, ShouldColorizeBars ? propertyData.ProbabilityItemColors[index] : Color.gray);
            }
        
            range.x = Mathf.Clamp(range.x, 0f, probability);
            range.y = Mathf.Clamp(range.y, probability, 1f);
            itemPropertyCache.p_InfluenceSpread.vector2Value = range;
        }

        internal static void DrawElement(PropertyData propertyData, Rect rect, int index, FloatLabelField floatLabelField, float itemRectAlignmentFixX)
        {
            var toggleRect = new Rect(rect.position.x, rect.position.y + 2f, 18f, EditorGUIUtility.singleLineHeight);
            var indexRect = new Rect(toggleRect.xMax, rect.position.y + 1f, 30f, EditorGUIUtility.singleLineHeight);
            
            var utilityRect = new Rect(indexRect.xMax + 5f, rect.position.y + 2f, rect.width * .6f * propertyData.DrawerSettings.ElementInfoSpace, EditorGUIUtility.singleLineHeight);
            
            var influenceProviderToggleRect = new Rect(utilityRect.xMax + 4f, rect.position.y + 2f, DrawInfluenceProviderToggle ? 16f : 0f, 16f);
            var probRect = new Rect(influenceProviderToggleRect.xMax + 4f, rect.position.y + 1f, 76f, EditorGUIUtility.singleLineHeight);
            var colorRect = new Rect(probRect.xMax + 4f, rect.position.y + 3f, 16f, 16f);

            var m_RemoveButtonRect = new Rect(rect.xMax - 25f, rect.position.y + 1f, 25f, EditorGUIUtility.singleLineHeight);
            var propertyWidth = Mathf.Abs(m_RemoveButtonRect.xMin - colorRect.xMax - 10f - itemRectAlignmentFixX);
            var itemRect = new Rect(colorRect.xMax + 4f + itemRectAlignmentFixX, rect.position.y + 2f, propertyWidth,
                propertyData.ItemPropertyCache[index].p_Value.isExpanded ? rect.height - 3f : EditorGUIUtility.singleLineHeight);
            
            var itemEnabled = EditorGUI.Toggle(toggleRect, propertyData.ItemPropertyCache[index].p_Enabled.boolValue);
            propertyData.ItemPropertyCache[index].p_Enabled.boolValue = itemEnabled;

            EditorGUI.LabelField(indexRect, propertyData.ItemInfoCache[index].index, style: itemEnabled ? PLDrawerTheme.ElementEnabledTextStyle : PLDrawerTheme.ElementDisabledTextStyle);
            
            // PREVIEW SLIDER / TEST RESULTS / INFLUENCE PREVIEW
            var valueIsInfluenceProvider = propertyData.ItemInfoCache[index].probabilityItemObject.ValueIsInfluenceProvider;
            var isInfluencedItem = propertyData.ItemInfoCache[index].probabilityItemObject.IsInfluencedItem;
            
            if (propertyData.TestResults.indexPicks.Count > 0)
            {
                if (propertyData.TestResults.indexPicks.TryGetValue(index, out var _value) && _value > 0)
                {
                    var pickCountResultRect = new Rect(utilityRect.x, utilityRect.y, utilityRect.width * .4f, EditorGUIUtility.singleLineHeight);
                    var percentageResultRect = new Rect(pickCountResultRect.xMax, pickCountResultRect.y, utilityRect.width * .6f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(pickCountResultRect, $"{_value.ToString("N0")} x", PLDrawerTheme.ElementEnabledPercentageStyle);

                    var color = PLDrawerTheme.NormalTextColor;
                    if (propertyData.DrawerSettings.ColorizeTestResults)
                    {
                        var percentage = ((float)_value / propertyData.TestResults.pickCount);
                        var difference = (propertyData.GetBaseProbability(index) - percentage) * propertyData.DrawerSettings.TestColorizeSensitivity * 7f;
                        color = RNGNStaticData.Settings.TestColorGradient.Evaluate(-difference + .5f);
                    }

                    PLDrawerTheme.TestResultPercentageStyle.normal.textColor = color;
                    EditorGUI.LabelField(percentageResultRect, $"{((float)_value / propertyData.TestResults.pickCount).ToString($"P{propertyData.DrawerSettings.TestPercentageDigits.ToString()}")}",
                        PLDrawerTheme.TestResultPercentageStyle);
                }
            }
            else
            {
                if (propertyData.IsInfluencedList || isInfluencedItem)
                {
                    var spreadMinValueRect = new Rect(utilityRect.x, utilityRect.y, 40f, EditorGUIUtility.singleLineHeight);
                    utilityRect.width -= 98f;
                    utilityRect.x = spreadMinValueRect.xMax + 4f;
                    var spreadMaxValueRect = new Rect(utilityRect.xMax + 4f, utilityRect.y, 50f, EditorGUIUtility.singleLineHeight);

                    // Colorize Spread Difference
                    PLDrawerTheme.SpreadValueStyle.normal.textColor = propertyData.DrawerSettings.ColorizeSpreadDifference ? RNGNStaticData.Settings.SpreadColorGradient.Evaluate(-((propertyData.GetBaseProbability(index) - propertyData.ItemInfoCache[index].InfluencedProbabilityLimits.x) * propertyData.DrawerSettings.SpreadColorizeSensitivity * 3f) + .5f) : PLDrawerTheme.NormalTextColor;
                    EditorGUI.LabelField(spreadMinValueRect, propertyData.ItemInfoCache[index].spreadMinPercentage, PLDrawerTheme.SpreadValueStyle);
                    PLDrawerTheme.SpreadValueStyle.normal.textColor = propertyData.DrawerSettings.ColorizeSpreadDifference ? RNGNStaticData.Settings.SpreadColorGradient.Evaluate(-((propertyData.GetBaseProbability(index) - propertyData.ItemInfoCache[index].InfluencedProbabilityLimits.y) * propertyData.DrawerSettings.SpreadColorizeSensitivity * 3f) + .5f) : PLDrawerTheme.NormalTextColor;
                    EditorGUI.LabelField(spreadMaxValueRect, propertyData.ItemInfoCache[index].spreadMaxPercentage, PLDrawerTheme.SpreadValueStyle);
                }
                
                EditorGUI.BeginChangeCheck();
                DrawUtilitySlider(propertyData, utilityRect, index, isInfluencedItem, 
                    propertyData.IsInfluencedList == false && propertyData.DrawerSettings.NormalizePreviewBars, 
                    propertyData.IsInfluencedList && propertyData.DrawerSettings.ShowSpreadOnBars);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyData.ItemPropertyCache[index].p_InfluenceSpread.serializedObject.ApplyModifiedProperties();
                    propertyData.SetSpreadCache();
                }
            }

            // SHOW INFLUENCE PROVIDER TOGGLE
            if (DrawInfluenceProviderToggle)
            {
                if (GUI.Button(influenceProviderToggleRect, string.Empty, propertyData.ItemInfoCache[index].InfluenceProviderExpanded ? PLDrawerTheme.PickCountsLinkedStyle : PLDrawerTheme.PickCountsUnlinkedStyle))
                {
                    var itemInfoCache = propertyData.ItemInfoCache[index];
                    itemInfoCache.InfluenceProviderExpanded = !itemInfoCache.InfluenceProviderExpanded;
                }
            }

            // DIRECT PROBABILITY INPUT
            var edit = floatLabelField.DrawAndHandleInput(probRect,
                index, 
                propertyData.GetBaseProbability(index),
                propertyData.ItemPropertyCache[index].p_Locked.boolValue,
                propertyData.ItemInfoCache[index].listElementPercentage,
                itemEnabled ? PLDrawerTheme.ElementEnabledPercentageStyle : PLDrawerTheme.ElementDisabledPercentageStyle);
            if (edit.Finished)
            {
                Undo.RecordObject(propertyData.p_ProbabilityListProperty.serializedObject.targetObject, $"Set Item {index} Probability");
                propertyData.ProbabilityListEditorInterface.SetItemBaseProbability(index, edit.Value);
                propertyData.SetupPropertiesRequired = true;
            }

            if (floatLabelField.SelectedIndex == index + 1) // Tab pressed
            {
                var nextIndex = index + 1;
                while (nextIndex < propertyData.ItemPropertyCache.Count && propertyData.ItemPropertyCache[nextIndex].p_Locked.boolValue) nextIndex++;
                floatLabelField.SelectedIndex = nextIndex < propertyData.ItemPropertyCache.Count ? nextIndex : -1;
            }

            EditorGUI.DrawRect(colorRect, PLDrawerTheme.DimBackgroundColor);
            EditorGUI.DrawRect(colorRect, propertyData.ProbabilityItemColors[index]);
            
            if(Event.current.type == EventType.Repaint) PLDrawerTheme.RectBorder.Draw(colorRect, false, false, false, false);

            if (GUI.Button(colorRect, string.Empty, propertyData.ItemPropertyCache[index].p_Locked.boolValue ? PLDrawerTheme.LockIconEnabled : PLDrawerTheme.LockIconHint))
                propertyData.ToggleItemLocked(index);

            if (propertyData.ValueIsGenericType)
            {
                if (propertyData.ItemInfoCache[index].ProviderType.HasAny(ItemProviderType.InfoProvider) && propertyData.ItemInfoCache[index].valueObject != null)
                {
                    m_TempContent.text = propertyData.ItemInfoCache[index].info;
                }
                else m_TempContent.text = propertyData.ItemPropertyCache[index].p_Value.type;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * .3f * (1.001f - propertyData.DrawerSettings.ElementInfoSpace);
                propertyData.ItemPropertyCache[index].p_Value.isExpanded = propertyData.ItemInfoCache[index].isExpandedProperty;
            }
            else
            {
                m_TempContent.text = string.Empty;
                EditorGUIUtility.labelWidth = 0;
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(itemRect, propertyData.ItemPropertyCache[index].p_Value, m_TempContent, propertyData.ValueIsGenericType);
            if (EditorGUI.EndChangeCheck())
            {
                propertyData.ValuesChangedFor = index;
                propertyData.p_ProbabilityItems.GetArrayElementAtIndex(index).serializedObject.ApplyModifiedProperties();
                propertyData.SetupPropertiesRequired = true;
                propertyData.ItemPropertyCache[index].p_Value.serializedObject.ApplyModifiedProperties();
                propertyData.ItemInfoCache[index].probabilityItemObject.UpdateProperties();
            }

            if (propertyData.ValueIsGenericType) propertyData.ItemInfoCache[index].isExpandedProperty = propertyData.ItemPropertyCache[index].p_Value.isExpanded;

            // Item Remove Button
            var onlyUnlockedItemWithProbability = propertyData.IndexOfUnremovableItem == index;
            if (GUI.Button(m_RemoveButtonRect, "X", onlyUnlockedItemWithProbability || propertyData.UnlockedItems == 0 ? PLDrawerTheme.OptionButtonDisabled : PLDrawerTheme.OptionButtonOn))
            {
                if (propertyData.UnlockedItems != 0) propertyData.RemoveItem(index);
                GUIUtility.ExitGUI();
            }
            
            // INFLUENCE PROVIDER FIELD
            if (propertyData.ItemInfoCache[index].InfluenceProviderExpanded && DrawInfluenceProviderToggle)
            {
                var influenceProviderRect = new Rect(indexRect.xMax + 5f, utilityRect.yMax + 4f, rect.width * .6f * propertyData.DrawerSettings.ElementInfoSpace, EditorGUIUtility.singleLineHeight);
                if(valueIsInfluenceProvider)
                {
                    EditorGUI.LabelField(influenceProviderRect, "Value is Influence Provider.", PLDrawerTheme.NormalTextStyle);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    propertyData.ItemPropertyCache[index].p_InfluenceProvider.objectReferenceValue = EditorGUI.ObjectField(influenceProviderRect,
                        propertyData.ItemPropertyCache[index].p_InfluenceProvider.objectReferenceValue, typeof(IProbabilityInfluenceProvider), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        propertyData.p_ProbabilityItems.GetArrayElementAtIndex(index).serializedObject.ApplyModifiedProperties();
                        propertyData.ItemInfoCache[index].probabilityItemObject.UpdateProperties();
                        propertyData.SetupPropertiesRequired = true;
                    }
                }

                if(isInfluencedItem)
                {
                    var influenceInfo = "-> " + propertyData.ItemInfoCache[index].probabilityItemObject.InfluenceProvider?.InfluenceInfo;
                    m_TempContent.text = influenceInfo;
                    propertyData.ItemInfoCache[index].influenceInfoHeight = PLDrawerTheme.LabelInfoStyle.CalcHeight(m_TempContent, influenceProviderRect.width);
                    var influenceInfoRect = new Rect(influenceProviderRect.x + 4f, influenceProviderRect.yMax, influenceProviderRect.width, propertyData.ItemInfoCache[index].influenceInfoHeight);
                    EditorGUI.LabelField(influenceInfoRect, influenceInfo, PLDrawerTheme.LabelInfoStyle);
                }
                else
                {
                    propertyData.ItemInfoCache[index].influenceInfoHeight = 0f;
                    m_TempContent.text = string.Empty;
                }
            }
        }
    }
}