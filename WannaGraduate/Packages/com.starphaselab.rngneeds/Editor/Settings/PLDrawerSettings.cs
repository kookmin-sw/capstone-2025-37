using System;
using UnityEngine;

namespace RNGNeeds.Editor
{
    [Serializable]
    internal class PLDrawerSettings
    {
        [HideInInspector] public string DrawerID;
        [HideInInspector] public long Created;
        [HideInInspector] public long Modified;
        [HideInInspector] public DrawerOptionSection DrawerOptionSection;
        
        [Header("Theme Options")]
        public string PalettePath;
        public bool DimColors;
        public bool Monochrome;
        [HideInInspector] public Color MonochromeColor;
        
        [Header("Info Displayed on the Stripe")]
        public bool ShowIndex;
        public bool ShowInfo;
        public bool ShowPercentage;

        [Header("Stripe Options")]
        public StripeHeight StripeHeight;
        [Range(0, 2)] public int StripePercentageDigits;
        
        [Header("List Entry Options")]
        [Range(0, 5)] public int ItemPercentageDigits;
        [Range(0, 5)] public int TestPercentageDigits;
        public bool ColorizePreviewBars;
        public bool NormalizePreviewBars;
        [Range(0f, 10f)] public float TestColorizeSensitivity;
        [Range(0f, 10f)] public float SpreadColorizeSensitivity;
        
        [Range(.2f, 1f)] public float ElementInfoSpace;

        [Header("Probability Influence Options")]
        public bool ShowInfluenceToggle;
        public bool ShowSpreadOnBars;
        [Range(0, 2)] public int SpreadPercentageDigits;
        
        public bool ColorizeTestResults => TestColorizeSensitivity > 0;
        public bool ColorizeSpreadDifference => SpreadColorizeSensitivity > 0;

        public void ApplySettings(PLDrawerSettings settings)
        {
            Modified = DateTime.Now.Ticks;
            ShowInfluenceToggle = settings.ShowInfluenceToggle;
            
            PalettePath = settings.PalettePath;
            ShowIndex = settings.ShowIndex;
            ShowInfo = settings.ShowInfo;
            ShowPercentage = settings.ShowPercentage;
            DimColors = settings.DimColors;
            Monochrome = settings.Monochrome;
            MonochromeColor = settings.MonochromeColor;
            StripeHeight = settings.StripeHeight;

            StripePercentageDigits = settings.StripePercentageDigits;
            ItemPercentageDigits = settings.ItemPercentageDigits;
            SpreadPercentageDigits = settings.SpreadPercentageDigits;
            TestPercentageDigits = settings.TestPercentageDigits;
            
            ElementInfoSpace = settings.ElementInfoSpace;
            
            ColorizePreviewBars = settings.ColorizePreviewBars;
            NormalizePreviewBars = settings.NormalizePreviewBars;

            TestColorizeSensitivity = settings.TestColorizeSensitivity;

            SpreadColorizeSensitivity = settings.SpreadColorizeSensitivity;
            ShowSpreadOnBars = settings.ShowSpreadOnBars;
        }
    }
}