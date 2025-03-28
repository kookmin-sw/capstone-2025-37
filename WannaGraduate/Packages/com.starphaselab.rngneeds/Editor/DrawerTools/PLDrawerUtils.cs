using System;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class PLDrawerUtils
    {
        private static readonly GUIContent ThemeButtonTempContent = new GUIContent();
        private static readonly GUIContent PickButtonTempContent = new GUIContent();
        
        public static float GetSubOptionsRectHeight(PLDrawerSettings drawerSettings, bool withAdvancedOptions)
        {
            switch (drawerSettings.DrawerOptionSection) // Set Sub Options rect height
            {
                case DrawerOptionSection.None:
                    return 0f;
                case DrawerOptionSection.Cog:
                    return withAdvancedOptions ? PLDrawerTheme.AdvancedOptionsTotalHeight + 52f: 58f;
                case DrawerOptionSection.Theme:
                case DrawerOptionSection.Stripe:
                    return 32f;
                case DrawerOptionSection.Picks:
                    return 58f;
                    // return withAdvancedOptions ? 58f : 32f;
            }

            return 0f;
        }

        public static GUIContent GetThemeSectionButtonContent(DrawerOptionsButtons drawerOptionsButtons, PLDrawerSettings drawerSettings)
        {
            switch (drawerOptionsButtons)
            {
                case DrawerOptionsButtons.Compact:
                    return drawerSettings.DrawerOptionSection == DrawerOptionSection.Theme ? PLDrawerContents.ThemeSectionButton : PLDrawerContents.ThemeSectionButtonCompact;
                case DrawerOptionsButtons.Full:
                    return PLDrawerContents.ThemeSectionButton;
                case DrawerOptionsButtons.Informative:
                    if (drawerSettings.Monochrome)
                    {
                        ThemeButtonTempContent.text = "Monochrome";
                        return ThemeButtonTempContent;
                    }
                    
                    var label = drawerSettings.PalettePath;
                    var lastIndex = label.LastIndexOf('/');

                    if (lastIndex != -1 && lastIndex + 1 < label.Length)
                    {
                        ThemeButtonTempContent.text = label.Substring(lastIndex + 1);
                        return ThemeButtonTempContent;
                    }
                    
                    ThemeButtonTempContent.text = label;
                    return ThemeButtonTempContent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawerOptionsButtons), drawerOptionsButtons, null);
            }
        }

        public static GUIContent GetStripeSectionButtonContent(DrawerOptionsButtons drawerOptionsButtons, PLDrawerSettings drawerSettings)
        {
            switch (drawerOptionsButtons)
            {
                case DrawerOptionsButtons.Compact:
                    return drawerSettings.DrawerOptionSection == DrawerOptionSection.Stripe ? PLDrawerContents.StripeSectionButton : PLDrawerContents.StripeSectionButtonCompact;
                case DrawerOptionsButtons.Full:
                    return PLDrawerContents.StripeSectionButton;
                case DrawerOptionsButtons.Informative:
                    return GetStripeHeightLabel(drawerSettings.StripeHeight);
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawerOptionsButtons), drawerOptionsButtons, null);
            }
        }

        public static GUIContent GetStripeHeightLabel(StripeHeight stripeHeight)
        {
            switch (stripeHeight)
            {
                case StripeHeight.Compact:
                    return PLDrawerContents.StripeHeightCompactLabel;
                case StripeHeight.Short:
                    return PLDrawerContents.StripeHeightShortLabel;
                case StripeHeight.Normal:
                    return PLDrawerContents.StripeHeightNormalLabel;
                case StripeHeight.Tall:
                    return PLDrawerContents.StripeHeightTallLabel;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stripeHeight), stripeHeight, null);
            }
        }

        public static GUIContent GetPickSectionButtonContent(DrawerOptionsButtons drawerOptionsButtons, PropertyData propertyData)
        {
            switch (drawerOptionsButtons)
            {
                case DrawerOptionsButtons.Compact:
                    return propertyData.DrawerSettings.DrawerOptionSection == DrawerOptionSection.Picks ? PLDrawerContents.PickSectionButton : PLDrawerContents.PickSectionButtonCompact;
                case DrawerOptionsButtons.Full:
                    return PLDrawerContents.PickSectionButton;
                case DrawerOptionsButtons.Informative:
                    var maintainInfo = propertyData.p_MaintainPickCountIfDisabled.boolValue ? "M " : "";
                    var preventRepeatInfo = "";
                    switch (propertyData.p_PreventRepeat.enumValueIndex)
                    {
                        case 0:
                            break;
                        case 1:
                        case 2:
                            preventRepeatInfo = $" {propertyData.p_PreventRepeat.enumDisplayNames[propertyData.p_PreventRepeat.enumValueIndex]}";
                            break;
                        case 3:
                            preventRepeatInfo = $" {propertyData.p_PreventRepeat.enumDisplayNames[propertyData.p_PreventRepeat.enumValueIndex]} ({propertyData.p_ShuffleIterations.intValue.ToString()})";
                            break;

                    }

                    PickButtonTempContent.text = propertyData.p_LinkPickCounts.boolValue
                        ? $"{maintainInfo}{propertyData.p_PickCountMin.intValue.ToString()}{preventRepeatInfo}"
                        : $"{maintainInfo}{propertyData.p_PickCountMin.intValue.ToString()} - {propertyData.p_PickCountMax.intValue.ToString()}{preventRepeatInfo}";
                    
                    return PickButtonTempContent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawerOptionsButtons), drawerOptionsButtons, null);
            }
        }
    }
}