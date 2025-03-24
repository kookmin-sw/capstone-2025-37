using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using StarphaseTools.Core;
using StarphaseTools.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class CacheTools
    {
        internal static void SetupProperties(this PropertyData propertyData)
        {
            propertyData.HoveredListElement = -1;
            propertyData.ModifierRects.Clear();
            propertyData.ProbabilityRects.Clear();
            propertyData.ItemPropertyCache.Clear();
            
            for (var i = 0; i < propertyData.p_ProbabilityItems.arraySize; i++)
            {
                propertyData.ProbabilityRects.Add(default);
                propertyData.ModifierRects.Add(default);
                var currentItemProperty = propertyData.p_ProbabilityItems.GetArrayElementAtIndex(i);
                var newItemProperty = new ItemPropertyCache
                {
                    p_Value = currentItemProperty.FindPropertyRelative("m_Value"),
                    p_Enabled = currentItemProperty.FindPropertyRelative("m_Enabled"),
                    p_Locked = currentItemProperty.FindPropertyRelative("m_Locked"),
                    p_InfluenceProvider = currentItemProperty.FindPropertyRelative("m_InfluenceProvider"),
                    p_InfluenceSpread = currentItemProperty.FindPropertyRelative("m_InfluenceSpread")
                };

                propertyData.ItemPropertyCache.Add(newItemProperty);
                propertyData.IsInfluencedList = propertyData.ProbabilityListEditorInterface.IsListInfluenced;

                if (i == 0)
                {
                    if (newItemProperty.p_Value == null)
                    {
                        if(propertyData.ValueCannotBeObtained == false) RLogger.Log($"The ProbabilityList named '{propertyData.NameOfProperty}' could not find the value associated with its type. If the type is a custom class, please ensure it is marked as serializable.", LogMessageType.Warning);
                        propertyData.ValueCannotBeObtained = true;
                        return;
                    }
                    
                    propertyData.ValueIsGenericType = newItemProperty.p_Value.propertyType == SerializedPropertyType.Generic;
                    propertyData.ValueIsObjectReference = newItemProperty.p_Value.propertyType == SerializedPropertyType.ObjectReference;
                }
            }

            propertyData.StripeColors = RNGNStaticData.Settings.GetColorsFromPalette(propertyData.DrawerSettings.PalettePath);
            propertyData.SetSelectionMethodInfo();

            propertyData.GetUnlockedItemsInfo();
            propertyData.SetupInfoCache();
            propertyData.SetupColors();
        }

        internal static void SetSelectionMethodInfo(this PropertyData propertyData)
        {
            var selectionMethod = RNGNeedsCore.GetSelectionMethod(propertyData.ProbabilityListEditorInterface.SelectionMethodID);
            propertyData.SelectionMethodName = selectionMethod.Name;
            propertyData.SelectionMethodTooltip = $"{PLDrawerContents.SelectionMethodTooltip}\n\nCurrent: {selectionMethod.Name}\n{selectionMethod.EditorTooltip}";
        }
        
        internal static void SetupColors(this PropertyData propertyData)
        {
            propertyData.ProbabilityItemColors.Clear();
            for (var i = 0; i < propertyData.p_ProbabilityItems.arraySize; i++)
            {
                var colorIndex = i <= propertyData.StripeColors.Count - 1 ? i : i - propertyData.StripeColors.Count * Mathf.FloorToInt(i / propertyData.StripeColors.Count);
                propertyData.ProbabilityItemColors.Add(
                    propertyData.ItemInfoCache[i].ProviderType.HasAny(ItemProviderType.ColorProvider) ?
                        ((IProbabilityItemColorProvider)propertyData.ItemInfoCache[i].valueObject).ItemColor : 
                    propertyData.DrawerSettings.Monochrome ? 
                    propertyData.DrawerSettings.MonochromeColor :
                    propertyData.ItemPropertyCache[i].p_Value.propertyType == SerializedPropertyType.Color ? 
                        propertyData.ItemPropertyCache[i].p_Value.colorValue :
                        propertyData.StripeColors[colorIndex]);
            }
        }

        internal static void SetupInfoCache(this PropertyData propertyData)
        {
            if (propertyData.p_ProbabilityItems.arraySize < propertyData.ItemInfoCache.Count)   // trim cache
            {
                for (var i = propertyData.p_ProbabilityItems.arraySize; i <= propertyData.ItemInfoCache.Count; i++)
                {
                    if (propertyData.ItemInfoCache.ContainsKey(i) == false) continue;
                    propertyData.ItemInfoCache.Remove(i);
                }
            }
                
            for (var i = 0; i < propertyData.p_ProbabilityItems.arraySize; i++)
            {
                if (propertyData.ItemInfoCache.TryGetValue(i, out _) == false) propertyData.ItemInfoCache.Add(i, new ItemInfoCache());

                propertyData.SetProviderTypeFor(i);
                propertyData.SetObjectFor(i);
                propertyData.SetInfoCacheFor(i);
                propertyData.SetPercentageCacheFor(i);
            }
            
            propertyData.SetSpreadCache();
        }

        internal static void SetProviderTypeFor(this PropertyData propertyData, int index)
        {
            propertyData.ItemInfoCache[index].ProviderType = propertyData.ProbabilityListEditorInterface.GetItemProviderTypes(index);
        }

        internal static void SetObjectFor(this PropertyData propertyData, int index)
        {
            var itemInfoCache = propertyData.ItemInfoCache[index];
            if (propertyData.ValueIsGenericType || propertyData.ValueIsObjectReference) itemInfoCache.valueObject = propertyData.ItemPropertyCache[index].p_Value.GetTargetObjectOfProperty();
            
            itemInfoCache.probabilityItemObject = propertyData.p_ProbabilityItems.GetArrayElementAtIndex(index).GetTargetObjectOfProperty() as IProbabilityItem;
            itemInfoCache.probabilityItemObject?.UpdateProperties();
        }

        internal static void SetSpreadCache(this PropertyData propertyData)
        {
            // calculate and normalize influenced list
            var influencedProbabilitiesLimits = propertyData.ProbabilityListEditorInterface.GetInfluencedProbabilitiesLimits;
            var percentageStyle = $"P{propertyData.DrawerSettings.SpreadPercentageDigits.ToString()}";
            for (var i = 0; i < propertyData.p_ProbabilityItems.arraySize; i++)
            {
                if (i >= influencedProbabilitiesLimits.Count) continue;
                var itemInfoCache = propertyData.ItemInfoCache[i];
                itemInfoCache.InfluencedProbabilityLimits = influencedProbabilitiesLimits[i];
                itemInfoCache.spreadMinPercentage = influencedProbabilitiesLimits[i].x.ToString(percentageStyle);
                itemInfoCache.spreadMaxPercentage = influencedProbabilitiesLimits[i].y.ToString(percentageStyle);
            }
        }

        internal static void SetInfoCacheFor(this PropertyData propertyData, int index)
        {
            var itemInfoCache = propertyData.ItemInfoCache[index];
            itemInfoCache.index = index.ToString();
            if(propertyData.CanDisplayItemInfo) itemInfoCache.info = propertyData.GetItemInfo(index);
        }
        
        internal static void SetPercentageCacheFor(this PropertyData propertyData, int index)
        {
            var itemInfoCache = propertyData.ItemInfoCache[index];
            itemInfoCache.stripePercentage = propertyData.GetBaseProbability(index).ToString($"P{propertyData.DrawerSettings.StripePercentageDigits.ToString()}", CultureInfo.InvariantCulture);
            itemInfoCache.listElementPercentage = propertyData.GetBaseProbability(index).ToString($"P{propertyData.DrawerSettings.ItemPercentageDigits.ToString()}", CultureInfo.InvariantCulture);
        }

        internal static bool IsCollection(string propertyPath) => propertyPath.Contains(".pl_collection.Array");
        internal static bool IsArrayAndNotCollection(string propertyPath) => propertyPath.Contains(".Array.") && IsCollection(propertyPath) == false;
        internal static bool IsArray(string propertyPath) => propertyPath.Contains(".Array.");
        
        internal static string GetNameOfProperty(SerializedProperty property, bool isArray)
        {
            if (isArray == false) return property.displayName;
            var match = Regex.Match(property.propertyPath, @"\[(\d+)\]");
            if (match.Success == false) return "#";
            var number = int.Parse(match.Groups[1].Value);
            return number.ToString().Length < 2 ? $"# 0{number.ToString()}" : $"# {number.ToString()}";
        }

        private static string GetItemInfo(this PropertyData propertyData, int index)
        {
            if (propertyData.ItemInfoCache[index].ProviderType.HasAny(ItemProviderType.InfoProvider))
                return propertyData.ItemInfoCache[index].valueObject == null ? "NULL" : ((IProbabilityItemInfoProvider)propertyData.ItemInfoCache[index].valueObject).ItemInfo;

            var item = propertyData.ItemPropertyCache[index].p_Value;

            if (item.isArray && item.propertyType != SerializedPropertyType.String) return $"[{item.arraySize.ToString()}]";
            
            switch (item.propertyType)
            {
                case SerializedPropertyType.ObjectReference: // Mono or SO if not InfoProvider
                    return (UnityEngine.Object)propertyData.ItemInfoCache[index].valueObject == null ? "NULL" : ((UnityEngine.Object)propertyData.ItemInfoCache[index].valueObject).name;
                case SerializedPropertyType.Generic: // Class
                    if (propertyData.ItemInfoCache[index].valueObject == null) return "NULL";
                    var itemObjectString = propertyData.ItemInfoCache[index].valueObject.ToString();
                    return itemObjectString.Substring(itemObjectString.LastIndexOf(".", StringComparison.Ordinal) + 1);
                case SerializedPropertyType.Integer:
                    var intValueType = item.GetTargetObjectOfProperty().GetType();
                    if (intValueType == typeof(int) || intValueType == typeof(short) || intValueType == typeof(byte)) return item.intValue.ToString(CultureInfo.InvariantCulture);
                    if (intValueType == typeof(long)) return item.longValue.ToString(CultureInfo.InvariantCulture);
                    if (intValueType == typeof(ushort)) return item.intValue.ToString(CultureInfo.InvariantCulture);;
                    
                    #if UNITY_2022_1_OR_NEWER
                    if (intValueType == typeof(uint)) return item.uintValue.ToString(CultureInfo.InvariantCulture);
                    if (intValueType == typeof(ulong)) return item.ulongValue.ToString(CultureInfo.InvariantCulture);
                    #else
                    if (intValueType == typeof(uint)) return item.longValue.ToString(CultureInfo.InvariantCulture);
                    if (intValueType == typeof(ulong)) return ((decimal)item.longValue).ToString(CultureInfo.InvariantCulture);
                    #endif
                    
                    break;
                case SerializedPropertyType.Float:
                    var floatValueType = item.GetTargetObjectOfProperty().GetType();
                    if (floatValueType == typeof(float)) return item.floatValue.ToString(CultureInfo.InvariantCulture);
                    if (floatValueType == typeof(double)) return item.doubleValue.ToString(CultureInfo.InvariantCulture);
                    break;
                case SerializedPropertyType.String:
                    return item.stringValue;
                case SerializedPropertyType.Boolean:
                    return item.boolValue.ToString();
                case SerializedPropertyType.Enum:
                    var enumType = item.GetTargetObjectOfProperty().GetType();
                    var isFlags = enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
                    #if UNITY_2021_1_OR_NEWER
                    return isFlags ? GetEnumFlagsNames(enumType, item.enumValueFlag) : item.enumValueIndex >= 0 && item.enumValueIndex < item.enumDisplayNames.Length ? item.enumDisplayNames[item.enumValueFlag] : "Missing ?";
                    #else
                    return isFlags ? string.Empty : item.enumValueIndex >= 0 && item.enumValueIndex < item.enumDisplayNames.Length ? item.enumDisplayNames[item.enumValueIndex] : "Missing ?";
                    #endif
                case SerializedPropertyType.Vector2:
                    return item.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return item.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return item.vector4Value.ToString();
                case SerializedPropertyType.Vector2Int:
                    return item.vector2IntValue.ToString();
                case SerializedPropertyType.Vector3Int:
                    return item.vector3IntValue.ToString();
                case SerializedPropertyType.Quaternion:
                    return item.quaternionValue.ToString();
                case SerializedPropertyType.LayerMask:
                    return GetLayerNames(item.intValue);
                case SerializedPropertyType.Rect:
                    return item.rectValue.ToString();
                case SerializedPropertyType.RectInt:
                    return item.rectIntValue.ToString();
                case SerializedPropertyType.Character:
                    return $"{((char)item.intValue).ToString()} ({item.intValue.ToString()})";
                case SerializedPropertyType.Bounds:
                    return item.boundsValue.ToString();
                case SerializedPropertyType.BoundsInt:
                    return item.boundsIntValue.ToString();
                #if UNITY_2021_1_OR_NEWER
                case SerializedPropertyType.Hash128:
                    return item.hash128Value.ToString();
                #endif
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.Color:
                    return string.Empty;
            }
            
            RLogger.Log($"GetItemInfo falls through switch to use GetTargetObjectOfProperty! {item.propertyType}", LogMessageType.Warning);
            return item.GetTargetObjectOfProperty()?.ToString();
        }
        
        private static string GetEnumFlagsNames(Type enumType, int enumValueIndex)
        {
            Enum enumValue = (Enum)Enum.ToObject(enumType, enumValueIndex);

            switch (enumValueIndex)
            {
                case 0:
                    return "None";
                case -1:
                    return "All";
                default:
                {
                    var names = enumValue.ToString().Split(',').Select(name => name.Trim()).ToArray();
                    return names.Length > 1 ? string.Join(" | ", names) : names[0];
                }
            }
        }

        private static string GetLayerNames(int layer)
        {
            switch (layer)
            {
                case -1:
                    return "All";
                case 0:
                    return "None";
            }

            var layerNames = string.Empty;
            for (var i = 0; i < 32; i++)
            {
                var shifted = 1 << i;
                if ((layer & shifted) != shifted) continue;
                var layerName = LayerMask.LayerToName(i);
                if(string.Equals(layerName, string.Empty) == false) layerNames = $"{layerNames} {layerName}";
            }
            return layerNames;
        }
    }
}