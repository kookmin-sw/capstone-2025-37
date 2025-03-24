using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class PLDrawerContents
    {
        public static readonly GUIContent ShowItemIndexButton = new GUIContent() { text = "#", tooltip = "Show Item index on Stripe." };
        public static readonly GUIContent ShowItemInfoButton = new GUIContent() { text = "i", tooltip = "Show Item info on Stripe." };
        public static readonly GUIContent CannotShowItemInfoButton = new GUIContent() { text = "i", tooltip = "This type cannot show info on Stripe." };
        public static readonly GUIContent ShowItemPercentageButton = new GUIContent() { text = "%", tooltip = "Show Item percentage on Stripe." };
        public static readonly GUIContent DimColorsButton = new GUIContent() { tooltip = "Dim colors by probability."};
        public static readonly GUIContent ColorizeBarsButton = new GUIContent() { tooltip = "Colorize probability bars."};
        public static readonly GUIContent NormalizeBarsButton = new GUIContent() { tooltip = "Normalize probability bars."};
        
        public static readonly GUIContent LinkPickCounts = new GUIContent() { tooltip = "Link Pick Count range to disable random pick counts." };
        public static readonly GUIContent UnlinkPickCounts = new GUIContent() { tooltip = "Unlink Pick Count range to enable random pick count. You can use curve to bias the resulting pick count." };
        
        public static readonly GUIContent TestButton = new GUIContent() { text = "TEST", tooltip = "Run test with specified settings. Will pick desired count of items using chosen Selection Method and Prevent Repeat options. Additional test results can be found in console.\n\nNote: Currently, Prevent Repeat options do not apply between tests. This is because the test results are not added to the list's history."};
        
        public static readonly GUIContent ClearTestResultsButton = new GUIContent() { text = "X", tooltip = "Clear test results."};
        public static readonly GUIContent AddItemButton = new GUIContent() { text = "ADD", tooltip = "Add new item to list."};
        public static readonly GUIContent SetAsDefaultSettingsButton = new GUIContent() { text = "Set as Default Settings (!)", tooltip = "All settings of this drawer will be set as Defaults in Preferences/RNGNeeds."};
        public static readonly GUIContent GetNewDrawerIDButton = new GUIContent() { text = "Get New Drawer ID", tooltip = "If you are duplicating objects, they may end up sharing drawer settings. Clicking this button will create new, independent settings for the current drawer."};
        public static readonly GUIContent ResetSettingsToDefaultButton = new GUIContent() { text = "Reset Settings to Default", tooltip = "Will reset settings of this drawer to Defaults found in Preferences/RNGNeeds."};
        public static readonly GUIContent ResetProbabilitiesButton = new GUIContent() { text = "Reset Probabilities", tooltip = "Will even out probabilities of unlocked items in list."};
        
        public static readonly GUIContent StripePercentageDigits = new GUIContent() { text = "Stripe % Digits", tooltip = "Number of percentage digits shown on the Stripe."};
        public static readonly GUIContent ItemPercentageDigits = new GUIContent() { text = "Item % Digits", tooltip = "Number of percentage digits shown for Item probability value."};
        public static readonly GUIContent TestPercentageDigits = new GUIContent() { text = "Test % Digits", tooltip = "Number of percentage digits shown for test results."};
        public static readonly GUIContent TestColorSensitivity = new GUIContent() { text = "Test Color Bias", tooltip = "Colorize strength of test results. Use higher setting to spot small differences."};
        
        public static readonly GUIContent InfluenceProviderButton = new GUIContent() { text = "Show Influence Toggle", tooltip = "Allow assigning Influence Provider to list items. If any item has provider assigned, the list is considered as 'influenced' and will always show the toggle."};
        public static readonly GUIContent SpreadPercentageDigits = new GUIContent() { text = "Spread % Digits", tooltip = "Number of percentage digits in Influence Spread values."};
        public static readonly GUIContent SpreadColorizeSensitivity = new GUIContent() { text = "Spread Color Bias", tooltip = "Colorize strength of Influence Spread values. Use higher setting to spot small differences."};
        public static readonly GUIContent ShowSpreadOnBars = new GUIContent() { text = "Show Spread on Bars", tooltip = "Influence spread will be shown on probability bars. Top part of bar will represent minimum spread, bottom part will show maximum spread."};
        
        public static readonly GUIContent ItemUtilitySpace = new GUIContent() { text = "Item Utility Space", tooltip = "The width of the list element reserved for item data and values."};

        public const string SelectionMethodTooltip = "Selection Method\nMathematical approach to selecting random item based on probability distribution. For most cases in game development the default 'Linear Search' is the ideal option.";
        public static readonly GUIContent MaintainPickCountButton = new GUIContent() { text = "Maintain Pick Count", tooltip = "If list has disabled Items, resulting pick count might be lower than desired. This option will respect pick count by continuing the selection process until the limit is reached. However, resulting probability distribution might be different."};
        public static readonly GUIContent SeedReadoutField = new GUIContent() { tooltip = "The last seed used by this list."};
        public static readonly GUIContent KeepSeedButton = new GUIContent() { text = "Keep Seed", tooltip = "If on, this list will not re-seed on each pick.\nBy default, each ProbabilityList will get new seed before each pick." };
        
        public static readonly GUIContent ThemeSectionButton = new GUIContent() { text = "THEME"};
        public static readonly GUIContent ThemeSectionButtonCompact = new GUIContent() { text = "T"};
        public static readonly GUIContent StripeSectionButton = new GUIContent() { text = "STRIPE"};
        public static readonly GUIContent StripeSectionButtonCompact = new GUIContent() { text = "S"};
        public static readonly GUIContent PickSectionButton = new GUIContent() { text = "PICK"};
        public static readonly GUIContent PickSectionButtonCompact = new GUIContent() { text = "P"};
        
        public static readonly GUIContent StripeHeightCompactLabel = new GUIContent() { text = "COMPACT"};
        public static readonly GUIContent StripeHeightShortLabel = new GUIContent() { text = "SHORT"};
        public static readonly GUIContent StripeHeightNormalLabel = new GUIContent() { text = "NORMAL"};
        public static readonly GUIContent StripeHeightTallLabel = new GUIContent() { text = "TALL"};
        
        // public static readonly GUIContent Template = new GUIContent() { };
    }
}