using UnityEditor;

namespace RNGNeeds.Editor
{
    internal static class ListItemTools
    {
        internal static void AddItem(this PropertyData propertyData)
        {
            Undo.RecordObject(propertyData.p_ProbabilityListProperty.serializedObject.targetObject, "Add New Item");
            propertyData.ProbabilityListEditorInterface.AddDefaultItem();
            propertyData.TestResults.Clear();
            EditorUtility.SetDirty(propertyData.p_ProbabilityListProperty.serializedObject.targetObject);
        }

        internal static void RemoveItem(this PropertyData propertyData, int index)
        {
            Undo.RecordObject(propertyData.p_ProbabilityListProperty.serializedObject.targetObject, $"Remove Item {index}");
            propertyData.ProbabilityListEditorInterface.RemoveItemAtIndex(index);
            propertyData.TestResults.Clear();
            EditorUtility.SetDirty(propertyData.p_ProbabilityListProperty.serializedObject.targetObject);
        }
        
        internal static bool IsItemEnabled(this PropertyData propertyData, int index)
        {
            return propertyData.ItemPropertyCache[index].p_Enabled.boolValue;
        }
        
        internal static void ToggleItemLocked(this PropertyData propertyData, int index)
        {
            propertyData.ItemPropertyCache[index].p_Locked.boolValue = !propertyData.ItemPropertyCache[index].p_Locked.boolValue;
            propertyData.p_ProbabilityItems.serializedObject.ApplyModifiedProperties();
            GetUnlockedItemsInfo(propertyData);
        }

        internal static void GetUnlockedItemsInfo(this PropertyData propertyData)
        {
            propertyData.IndexOfUnremovableItem = propertyData.ProbabilityListEditorInterface.IndexOfUnremovableItem;
            propertyData.UnlockedItems = propertyData.ProbabilityListEditorInterface.UnlockedItemsCount;
        }
    }
}