using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class DropdownMenus
    {
        internal static void SetupSelectionMethodDropdownMenu(PropertyData propertyData)
        {
            propertyData.SelectionMethodDropdownMenu = new GenericMenu();
            var methods = RNGNeedsCore.RegisteredSelectionMethods;
            foreach (var method in methods)
            {
                propertyData.SelectionMethodDropdownMenu.AddItem(new GUIContent(method.Name), propertyData.SelectionMethodName == method.Name, () =>
                {
                    Undo.RecordObject(propertyData.p_ProbabilityListProperty.serializedObject.targetObject, "Change Selection Method");
                    propertyData.ProbabilityListEditorInterface.SelectionMethodID = method.Identifier;
                    propertyData.SetSelectionMethodInfo();
                });
            }

            propertyData.SelectionMethodDropdownMenu.AddSeparator("");

            for (var i = 0; i < propertyData.p_PreventRepeat.enumDisplayNames.Length; i++)
            {
                var selectIndex = i;
                if (i == propertyData.p_PreventRepeat.enumDisplayNames.Length - 1)
                {
                    for (var iterations = 1; iterations <= 5; iterations++)
                    {
                        var selectedIterations = iterations;
                        propertyData.SelectionMethodDropdownMenu.AddItem(new GUIContent($"Prevent Repeat/{propertyData.p_PreventRepeat.enumDisplayNames[i]}/{iterations} Iterations"),
                            propertyData.p_PreventRepeat.enumValueIndex == i && propertyData.p_ShuffleIterations.intValue == iterations,
                            () =>
                            {
                                propertyData.p_PreventRepeat.enumValueIndex = selectIndex;
                                propertyData.p_PreventRepeat.serializedObject.ApplyModifiedProperties();
                                propertyData.p_ShuffleIterations.intValue = selectedIterations;
                                propertyData.p_ShuffleIterations.serializedObject.ApplyModifiedProperties();
                            });
                    }
                }
                else
                    propertyData.SelectionMethodDropdownMenu.AddItem(new GUIContent($"Prevent Repeat/{propertyData.p_PreventRepeat.enumDisplayNames[i]}"),
                        propertyData.p_PreventRepeat.enumValueIndex == i,
                        () =>
                        {
                            propertyData.p_PreventRepeat.enumValueIndex = selectIndex;
                            propertyData.p_PreventRepeat.serializedObject.ApplyModifiedProperties();
                        });
            }

            var iterationsInfo = propertyData.p_PreventRepeat.enumValueIndex == propertyData.p_PreventRepeat.enumDisplayNames.Length - 1
                ? $" ({propertyData.p_ShuffleIterations.intValue.ToString()} Iterations)"
                : string.Empty;
            propertyData.SelectionMethodDropdownMenu.AddDisabledItem(
                new GUIContent($" -> {propertyData.p_PreventRepeat.enumDisplayNames[propertyData.p_PreventRepeat.enumValueIndex]}{iterationsInfo}"));
        }

        internal static void SetupPaletteDropdownMenu(PropertyData propertyData)
        {
            propertyData.PaletteDropdownMenu = new GenericMenu();
            foreach (var palettePath in RNGNStaticData.Settings.GetColorPalettePaths())
            {
                propertyData.PaletteDropdownMenu.AddItem(new GUIContent(palettePath), propertyData.DrawerSettings.Monochrome == false && palettePath == propertyData.DrawerSettings.PalettePath, () =>
                {
                    propertyData.DrawerSettings.PalettePath = palettePath;
                    propertyData.DrawerSettings.Monochrome = false;
                    propertyData.StripeColors = RNGNStaticData.Settings.GetColorsFromPalette(propertyData.DrawerSettings.PalettePath);
                    propertyData.SetupColors();
                });
            
                if (palettePath == "Default") propertyData.PaletteDropdownMenu.AddSeparator("");
            }
            
            propertyData.PaletteDropdownMenu.AddSeparator("");
            propertyData.PaletteDropdownMenu.AddItem(new GUIContent("Monochrome"), propertyData.DrawerSettings.Monochrome, () =>
            {
                propertyData.DrawerSettings.Monochrome = true;
                propertyData.DrawerSettings.DimColors = true;
                propertyData.SetupColors();
            });
        }
    }
}