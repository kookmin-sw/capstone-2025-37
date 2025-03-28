using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class PLDrawerTheme
    {
        private static Texture2D OptionsRectTexture { get; set; }
        private static Texture2D CogIconTexture { get; set; }
        private static Texture2D CogHintIconTexture { get; set; }
        private static Texture2D StripeBorderTexture { get; set; }
        private static Texture2D OptionButtonOnTexture { get; set; }
        private static Texture2D OptionButtonOnHoverTexture { get; set; }
        private static Texture2D OptionButtonOffTexture { get; set; }
        private static Texture2D OptionButtonOffHoverTexture { get; set; }
        private static Texture2D SectionButtonOnTexture { get;  set; }
        private static Texture2D SectionButtonHoverTexture { get;  set; }
        private static Texture2D SectionButtonOffTexture { get; set; }
        private static Texture2D CriticalButtonHoverTexture { get; set; }
        private static Texture2D SeparatorTexture { get; set; }
        private static Texture2D MenuArrowTexture { get; set; }
        private static Texture2D LinkIconTexture { get; set; }
        private static Texture2D LinkHintIconTexture { get; set; }
        private static Texture2D LockIconTexture { get; set; }
        private static Texture2D LockHintIconTexture { get; set; }
        private static Texture2D InputFieldTexture { get; set; }
        
        private static Texture2D ColorizeBarsButtonOnTexture { get; set; }
        private static Texture2D ColorizeBarsButtonHoverTexture { get; set; }
        private static Texture2D ColorizeBarsButtonOffTexture { get; set; }
        
        private static Texture2D NormalizeBarsButtonOnTexture { get; set; }
        private static Texture2D NormalizeBarsButtonHoverTexture { get; set; }
        private static Texture2D NormalizeBarsButtonOffTexture { get; set; }
        
        private static Texture2D DimColorsButtonOnTexture { get; set; }
        private static Texture2D DimColorsButtonHoverTexture { get; set; }
        private static Texture2D DimColorsButtonOffTexture { get; set; }
        private static Texture2D RectBorderTexture { get; set; }
        private static Texture2D RectBorderAccentTexture { get; set; }
        private static Texture2D RectBorderDisabledTexture { get; set; }
        
        public static Texture2D LinkRectTexture { get; set; }
        
        private static Texture2D CollectionFrameTexture { get; set; }
        private static Texture2D CollectionHeaderTexture { get; set; }
        
        public static readonly GUIStyle StripeBorder;
        public static readonly GUIStyle CogIconOn;
        public static readonly GUIStyle CogIconOff;
        public static readonly GUIStyle ElementEnabledPercentageStyle;
        public static readonly GUIStyle ElementDisabledPercentageStyle;
        public static readonly GUIStyle ElementEnabledTextStyle;
        public static readonly GUIStyle ElementDisabledTextStyle;
        public static readonly GUIStyle TestResultPercentageStyle;
        public static readonly GUIStyle NormalTextStyle;
        public static readonly GUIStyle AdvancedOptionsLabelStyle;
        public static readonly GUIStyle LabelInfoStyle;
        public static readonly GUIStyle PropertyNameStyle;
        public static readonly GUIStyle CollectionPropertyNameStyle;
        public static readonly GUIStyle OptionButtonOn;
        public static readonly GUIStyle OptionButtonOff;
        public static readonly GUIStyle OptionButtonDisabled;
        public static readonly GUIStyle SectionButtonOn;
        public static readonly GUIStyle SectionButtonOff;
        public static readonly GUIStyle CriticalActionButton;
        public static readonly GUIStyle Separator;
        public static readonly GUIStyle MenuArrow;
        public static readonly GUIStyle LockIconEnabled;
        public static readonly GUIStyle LockIconDisabled;
        public static readonly GUIStyle LockIconHint;
        public static readonly GUIStyle PickCountsLinkedStyle;
        public static readonly GUIStyle PickCountsUnlinkedStyle;
        public static readonly GUIStyle InputField;
        public static readonly GUIStyle SpreadValueStyle;
        public static readonly GUIStyle OptionRectImage;
        public static readonly GUIStyle RectBorder;
        public static readonly GUIStyle RectBorderAccent;
        public static readonly GUIStyle RectBorderDisabled;
        
        public static readonly GUIStyle ColorizeBarsButtonOn;
        public static readonly GUIStyle ColorizeBarsButtonOff;
        
        public static readonly GUIStyle NormalizeBarsButtonOn;
        public static readonly GUIStyle NormalizeBarsButtonOff;
        
        public static readonly GUIStyle DimColorsButtonOn;
        public static readonly GUIStyle DimColorsButtonOff;

        public static readonly GUIStyle CollectionFrameImage;
        public static readonly GUIStyle CollectionHeaderImage;
        
        public static readonly Color NormalTextColor;
        public static readonly Color ModifierRectColor;
        public static readonly Color ProbabilityRectHighlightColor;
        public static readonly Color CollectionSeparatorColor;
        
        public static readonly Color ElementAltColor;
        public static readonly Color DimBackgroundColor;
        public static readonly Color SliderBackgroundColor;
        public static readonly Color SliderDisabledBackgroundColor;
        public static readonly Color SliderHandleColor;
        public static readonly Color SliderHandleDisabledColor;

        public const float CompactStripeHeight = 26f;
        public const float ShortStripeHeight = 34f;
        public const float NormalStripeHeight = 50f;
        public const float TallStripeHeight = 62f;

        public static readonly float[] AdvancedOptionsRowHeights;
        public static readonly float AdvancedOptionsTotalHeight;
        public const float AdvancedOptionsHPadding = 10f;
        public const float AdvancedOptionsVPadding = 5f;
        
        public const int versionSpecificFontSize =
                #if UNITY_2023_1_OR_NEWER
                                11;
                #else
                                12;
                #endif

        public const string hintButtonText =
                #if UNITY_2023_1_OR_NEWER
                                "?";
                #else
                                " ?";
                #endif

        private static Texture2D LoadTexture(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>($"{RNGNStaticData.PathToEditorAssets}{fileName}.png");
        }
        
        static PLDrawerTheme()
        {
            AdvancedOptionsRowHeights = new float[] { 
                20f, 
                20f, 
                1f,
                20f,
                20f,
                20f,
                20f,
                1f,
                20f,
            };
            
            AdvancedOptionsTotalHeight = AdvancedOptionsRowHeights.Sum();

            OptionsRectTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_OptionsRect_00000" : "PL_OptionsRectLight_00000");
            CollectionFrameTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_CollectionFrame_00000" : "PL_CollectionFrameLight_00000");
            CollectionHeaderTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_CollectionHeader_00000" : "PL_CollectionHeaderLight_00000");
            CogIconTexture = LoadTexture("PL_CogIcon_00000");
            
            CogHintIconTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_CogHintIcon_00000" : "PL_CogHintIcon_00000");
            ColorizeBarsButtonOnTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_ColorizeBarsButtonOn_00000" : "PL_ColorizeBarsButtonOn_00000");
            ColorizeBarsButtonHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_ColorizeBarsButtonHover_00000" : "PL_ColorizeBarsButtonHover_00000");
            ColorizeBarsButtonOffTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_ColorizeBarsButtonOff_00000" : "PL_ColorizeBarsButtonOff_00000");
            
            NormalizeBarsButtonOnTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_NormalizeBarsButtonOn_00000" : "PL_NormalizeBarsButtonOn_00000");
            NormalizeBarsButtonHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_NormalizeBarsButtonHover_00000" : "PL_NormalizeBarsButtonHover_00000");
            NormalizeBarsButtonOffTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_NormalizeBarsButtonOff_00000" : "PL_NormalizeBarsButtonOff_00000");
            
            DimColorsButtonOnTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_DimColorsButtonOn_00000" : "PL_DimColorsButtonOn_00000");
            DimColorsButtonHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_DimColorsButtonHover_00000" : "PL_DimColorsButtonHover_00000");
            DimColorsButtonOffTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_DimColorsButtonOff_00000" : "PL_DimColorsButtonOff_00000");
            
            StripeBorderTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_StripeBorder_00000" : "PL_StripeBorderLight_00000");
            OptionButtonOnTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_OptionButtonOn_00000" : "PL_OptionButtonOn_00000");
            OptionButtonOnHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_OptionButtonOnHover_00000" : "PL_OptionButtonOnHover_00000");
            OptionButtonOffTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_OptionButtonOff_00000" : "PL_OptionButtonOff_00000");
            OptionButtonOffHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_OptionButtonOffHover_00000" : "PL_OptionButtonOffHover_00000");
            
            SectionButtonOnTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_SectionButtonOn_00000" : "PL_SectionButtonOn_00000");
            SectionButtonHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_SectionButtonHover_00000" : "PL_SectionButtonHover_00000");
            SectionButtonOffTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_SectionButtonOff_00000" : "PL_SectionButtonOff_00000");
            CriticalButtonHoverTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_CriticalButtonHover_00000" : "PL_CriticalButtonHover_00000");
            InputFieldTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_InputField_00000" : "PL_InputField_00000");
            SeparatorTexture = LoadTexture("PL_Separator_00000");
            MenuArrowTexture = LoadTexture(EditorGUIUtility.isProSkin ? "PL_MenuArrow_00000" : "PL_MenuArrow_00000");
            LinkIconTexture = LoadTexture("PL_LinkIcon_00000");
            LinkHintIconTexture = LoadTexture("PL_LinkHintIcon_00000");
            LockIconTexture = LoadTexture("PL_LockIcon_00000");
            LockHintIconTexture = LoadTexture("PL_LockHintIcon_00000");
            LinkRectTexture = LoadTexture("PL_LockedStripeFG_00000");
            LinkRectTexture.wrapMode = TextureWrapMode.Repeat;
            RectBorderTexture = LoadTexture("PL_RectBorder_00000");
            RectBorderAccentTexture = LoadTexture("PL_RectBorderBlue_00000");
            RectBorderDisabledTexture = LoadTexture("PL_RectBorderGray_00000");
            
            OptionRectImage = new GUIStyle() { normal = { background = OptionsRectTexture }, border = new RectOffset(12, 12, 12, 12) };
            CollectionFrameImage = new GUIStyle() { normal = { background = CollectionFrameTexture }, border = new RectOffset(12, 12, 12, 12) };
            CollectionHeaderImage = new GUIStyle() { normal = { background = CollectionHeaderTexture }, border = new RectOffset(12, 12, 12, 12) };

            CogIconOn = new GUIStyle() { normal = { background = CogIconTexture } };
            CogIconOff = new GUIStyle() { normal = { background = CogHintIconTexture }, hover = { background = CogIconTexture}};

            ColorizeBarsButtonOn = new GUIStyle() { normal = { background = ColorizeBarsButtonOnTexture } };
            ColorizeBarsButtonOff = new GUIStyle() { normal = { background = ColorizeBarsButtonOffTexture }, hover = { background = ColorizeBarsButtonHoverTexture }};
            
            NormalizeBarsButtonOn = new GUIStyle() { normal = { background = NormalizeBarsButtonOnTexture } };
            NormalizeBarsButtonOff = new GUIStyle() { normal = { background = NormalizeBarsButtonOffTexture }, hover = { background = NormalizeBarsButtonHoverTexture }};
            
            DimColorsButtonOn = new GUIStyle() { normal = { background = DimColorsButtonOnTexture } };
            DimColorsButtonOff = new GUIStyle() { normal = { background = DimColorsButtonOffTexture }, hover = { background = DimColorsButtonHoverTexture }};
            
            StripeBorder = new GUIStyle { normal = { background = StripeBorderTexture }, border = new RectOffset(8, 8, 8, 8) };

            // Font Styles
            ElementEnabledTextStyle = new GUIStyle { fontSize = 12, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(.85f, .85f, .85f, 1f) : new Color(.1f, .1f, .1f, 1f) }, alignment = TextAnchor.MiddleCenter };
            ElementDisabledTextStyle = new GUIStyle() { fontSize = 12, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(.5f, .5f, .5f, 1f) : new Color(.45f, .45f, .45f, 1f) }, alignment = TextAnchor.MiddleCenter };
            ElementEnabledPercentageStyle = new GUIStyle(ElementEnabledTextStyle) { alignment = TextAnchor.MiddleRight };
            ElementDisabledPercentageStyle = new GUIStyle(ElementDisabledTextStyle) { alignment = TextAnchor.MiddleRight };

            TestResultPercentageStyle = new GUIStyle() { fontSize = 12, normal = { textColor = Color.yellow }, alignment = TextAnchor.MiddleRight };
            NormalTextColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            NormalTextStyle = new GUIStyle() { fontSize = 12, normal = { textColor = NormalTextColor } };
            AdvancedOptionsLabelStyle = new GUIStyle() { fontSize = 12, normal = { textColor = new Color(.8f, .8f, .8f, 1f) }, alignment = TextAnchor.MiddleLeft};
            LabelInfoStyle = new GUIStyle() { fontSize = 12, wordWrap = true, alignment = TextAnchor.UpperLeft, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(.8f, .8f, .8f, 1f) : new Color(.2f, .2f, .2f, 1f) } };
            PropertyNameStyle = new GUIStyle() { alignment = TextAnchor.MiddleRight, padding = new RectOffset(0, 60, 0, 0), fontSize = versionSpecificFontSize, fontStyle = FontStyle.Bold, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(.8f, .8f, .8f, 1f) : new Color(.9f, .9f, .9f, 1f) } };
            CollectionPropertyNameStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = versionSpecificFontSize, fontStyle = FontStyle.Bold, normal = { textColor = EditorGUIUtility.isProSkin ? new Color(.8f, .8f, .8f, 1f) : new Color(.9f, .9f, .9f, 1f) } };
            
            OptionButtonOn = new GUIStyle()
            {
                fontSize = versionSpecificFontSize,
                // normal = { background = OptionButtonOnTexture, textColor = EditorGUIUtility.isProSkin ? new Color(.8f, .8f, .8f, 1f) : new Color(.35f, .35f, .35f, 1f) },
                normal = { background = OptionButtonOnTexture, textColor = Color.white },
                hover = { background = OptionButtonOnHoverTexture, textColor = Color.white},
                border = new RectOffset(8, 8, 8, 8), alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold,
                active = { background = SectionButtonOnTexture, textColor = Color.white }
            };
            
            OptionButtonOff = new GUIStyle(OptionButtonOn) { normal = { background = OptionButtonOffTexture, textColor = new Color(.75f, .75f, .75f, 1f) }, hover = { background = OptionButtonOffHoverTexture, textColor = Color.white}, };
            OptionButtonDisabled = new GUIStyle(OptionButtonOff) { normal = { background = OptionButtonOffTexture, textColor = EditorGUIUtility.isProSkin ? new Color(.6f, .6f, .6f, 1f) : new Color(.5f, .5f, .5f, 1f) }, hover = { background = OptionButtonOffTexture, textColor = new Color(.6f, .6f, .6f, 1f)} };
            SectionButtonOn = new GUIStyle(OptionButtonOn) { normal = { background = SectionButtonOnTexture, textColor = Color.white } };
            SectionButtonOff = new GUIStyle(OptionButtonOff) { normal = { background = SectionButtonOffTexture }, hover = { background = SectionButtonHoverTexture } };
            CriticalActionButton = new GUIStyle(OptionButtonOff) { hover = { background = CriticalButtonHoverTexture } };
            InputField = new GUIStyle() { fontSize = versionSpecificFontSize, normal = { background = InputFieldTexture, textColor = Color.white }, border = new RectOffset(8, 8, 8, 8), alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, };

            RectBorder = new GUIStyle() { normal = { background = RectBorderTexture }, border = new RectOffset(4, 4, 4, 4) };
            RectBorderAccent = new GUIStyle() { normal = { background = RectBorderAccentTexture }, border = new RectOffset(4, 4, 4, 4) };
            RectBorderDisabled = new GUIStyle() { normal = { background = RectBorderDisabledTexture }, border = new RectOffset(4, 4, 4, 4) };
            
            Separator = new GUIStyle() { normal = { background = SeparatorTexture } };
            MenuArrow = new GUIStyle() { normal = { background = MenuArrowTexture } };
            LockIconHint = new GUIStyle() { hover = { background = LockHintIconTexture } };
            LockIconEnabled = new GUIStyle() { normal = { background = LockIconTexture } };
            LockIconDisabled = new GUIStyle() { normal = { background = LockHintIconTexture } };
            PickCountsLinkedStyle = new GUIStyle() { normal = { background = LinkIconTexture } };
            PickCountsUnlinkedStyle = new GUIStyle() { normal = { background = LinkHintIconTexture }, hover = { background = LinkIconTexture } };
            SpreadValueStyle = new GUIStyle() { alignment = TextAnchor.MiddleRight, fontSize = 11, normal = { textColor = Color.white } };
            
            ModifierRectColor = new Color(1f, 1f, 1f, .6f);
            ProbabilityRectHighlightColor = new Color(1f, 1f, 1f, .35f);
            CollectionSeparatorColor = EditorGUIUtility.isProSkin ? new Color(.7f, .7f, .7f, 1f) : new Color(.35f, .35f, .35f, 1f);
            
            ElementAltColor = EditorGUIUtility.isProSkin ? new Color(.29f, .29f, .29f, 1f) : new Color(.73f, .73f, .73f, 1f);
            SliderBackgroundColor = new Color(.30f, .36f, .42f, 1f);
            SliderDisabledBackgroundColor = new Color(.25f, .29f, .33f, 1f);
            SliderHandleColor = new Color(.54f, .8f, .88f, 1f);
            SliderHandleDisabledColor = new Color(.54f, .8f, .88f, .5f);

            DimBackgroundColor = EditorGUIUtility.isProSkin ? new Color(.22f, .22f, .22f, 1f) : new Color(.35f, .35f, .35f, 1f);
        }
    }
}