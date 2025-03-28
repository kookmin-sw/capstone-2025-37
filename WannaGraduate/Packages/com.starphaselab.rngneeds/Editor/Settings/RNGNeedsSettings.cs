using System;
using System.Collections.Generic;
using StarphaseTools.Core;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    [FilePath("ProjectSettings/RNGNeedsSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class RNGNeedsSettings : ScriptableSingleton<RNGNeedsSettings>
    {
        public DrawerOptionsLevel DrawerOptionsLevel => m_DrawerOptionsLevel;
        public DrawerOptionsButtons DrawerOptionButtons => m_DrawerOptionButtons;
        public InspectorRefreshMode InspectorRefreshMode => m_InspectorRefreshMode;
        public bool InvertScrollDirection => m_InvertScrollDirection;
        public Gradient TestColorGradient => EditorGUIUtility.isProSkin ? m_TestColorGradient : m_TestColorGradientLight;
        public Gradient SpreadColorGradient => EditorGUIUtility.isProSkin ? m_SpreadColorGradient : m_SpreadColorGradientLight;

        public LogMessageType EditorLogLevel => m_EditorLogLevel;
        public bool AllowLogColors => m_AllowLogColors;

        #region Internal
        
        internal static bool DevMode { get; set; }
        
        [InitializeOnLoadMethod]
        private static void SaveOnQuit()
        {
            EditorApplication.quitting += OnQuitting;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnQuitting()
        {
            instance.Save(true);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode) instance.Save(true);
        }
        
        private void OnEnable()
        {
            if (m_Initialized)
            {
                SetLoggerSettings();
                return;
            }
            
            m_Initialized = true;
            SetDefaultColorPalettes();
            SetGlobalDefaults();
        }

        internal PLDrawerSettings GetOrCreateDrawerSettings(string drawerID, bool ignoreSavedSettings = false)
        {
            foreach (var drawerSettings in DrawerSettings)
                if (drawerSettings.DrawerID.Equals(drawerID))
                {
                    drawerSettings.Modified = DateTime.Now.Ticks;
                    return drawerSettings;
                }
            
            if(ignoreSavedSettings == false)
            {
                var savedSettings = Resources.Load<TextAsset>($"plds_{drawerID}");
                if (savedSettings != null && string.IsNullOrEmpty(savedSettings.text) == false)
                {
                    var settings = JsonUtility.FromJson<PLDrawerSettings>(savedSettings.text);
                    DrawerSettings.Add(settings);
                    RLogger.Log($"Adding Saved Drawer Settings for {drawerID}", LogMessageType.Debug);
                    return settings;
                }
            }

            RLogger.Log($"Adding New Drawer Settings for {drawerID}", LogMessageType.Debug);
            var newDrawerSettings = new PLDrawerSettings();
            newDrawerSettings.Created = DateTime.Now.Ticks;
            newDrawerSettings.Modified = DateTime.Now.Ticks;
            newDrawerSettings.DrawerID = drawerID;
            newDrawerSettings.ApplySettings(m_DefaultDrawerSettings);
            newDrawerSettings.MonochromeColor = EditorGUIUtility.isProSkin ? m_DefaultMonochromeColor : m_DefaultMonochromeColorLight;
            DrawerSettings.Add(newDrawerSettings);
            Save(true);
            return newDrawerSettings;
        }

        #endregion

        [SerializeField] private bool m_Initialized;
        
        // Global Settings
        [Header("Options")] 
        [Tooltip("Advanced will show additional options in the drawer.")]
        [SerializeField] private DrawerOptionsLevel m_DrawerOptionsLevel = DrawerOptionsLevel.Basic;
        [Tooltip("Show Full, Compact or Informative section buttons in drawer Header. Use Compact if you have limited inspector space.")]
        [SerializeField] private DrawerOptionsButtons m_DrawerOptionButtons = DrawerOptionsButtons.Full;
        [Tooltip("To achieve responsive visual-authoring drawer, the inspector has to repaint frequently. If you are experiencing editor slowdowns, try 'Optimized' option, which will repaint only during mouse drag. However, other actions, such as changing settings might feel less responsive.")]
        [SerializeField] private InspectorRefreshMode m_InspectorRefreshMode = InspectorRefreshMode.Responsive;

        [Header("Stripe Options")]
        [Tooltip("Use this to compensate system settings. For example on MacOS, the 'natural' scroll direction might feel inversed when using scroll to adjust probabilities.")]
        [SerializeField] private bool m_InvertScrollDirection;
        
        [Header("Dark Editor Theme Preferences")]
        [Tooltip("This will become the default Monochrome color for every new drawer when initialized while Dark Editor theme is active.")]
        [SerializeField] [ColorUsage(false, false)] private Color m_DefaultMonochromeColor;
        
        [Space(3f)]
        [Tooltip("When using Dark Editor theme, this gradient will be used to colorize test result variation from desired probability.")]
        [SerializeField] [GradientUsage(false)] private Gradient m_TestColorGradient = new Gradient();
        
        [Space(3f)]
        [Tooltip("When using Dark Editor theme, this gradient will be used to colorize probability spread in an influenced list.")]
        [SerializeField] [GradientUsage(false)] private Gradient m_SpreadColorGradient = new Gradient();
        
        [Header("Light Editor Theme Preferences")]
        [Tooltip("This will become the default Monochrome color for every new drawer when initialized while Light Editor theme is active.")]
        [SerializeField] [ColorUsage(false, false)] private Color m_DefaultMonochromeColorLight;
        
        [Space(3f)]
        [Tooltip("When using Light Editor theme, this gradient will be used to colorize test result variation from desired probability.")]
        [SerializeField] [GradientUsage(false)] private Gradient m_TestColorGradientLight = new Gradient();
        
        [Tooltip("Select the message types to be displayed in the editor console.")]
        [SerializeField] private LogMessageType m_EditorLogLevel = LogMessageType.Info | LogMessageType.Hint;
        [Tooltip("Enable or disable color-coded messages in the editor console.")]
        [SerializeField] private bool m_AllowLogColors = true;
        
        [Space(3f)]
        [Tooltip("When using Light Editor theme, this gradient will be used to colorize probability spread in influenced list.")]
        [SerializeField] [GradientUsage(false)] private Gradient m_SpreadColorGradientLight = new Gradient();

        [Tooltip("New drawers will be initialized with these settings.")]
        [SerializeField] internal PLDrawerSettings m_DefaultDrawerSettings;
        
        [HideInInspector] [SerializeField] internal List<PLDrawerSettings> DrawerSettings = new List<PLDrawerSettings>();

        public List<ProbabilityListColorPalette> colorPalettes = new List<ProbabilityListColorPalette>();

        private void SetGlobalDefaults()
        {
            m_DefaultDrawerSettings = new PLDrawerSettings();
            m_DefaultDrawerSettings.DrawerID = "DefaultSettings";
            m_DefaultDrawerSettings.ApplySettings(RNGNStaticData.DefaultDrawerSettings);

            m_DrawerOptionsLevel = DrawerOptionsLevel.Basic;
            m_DrawerOptionButtons = DrawerOptionsButtons.Full;
            m_InspectorRefreshMode = InspectorRefreshMode.Responsive;
            m_InvertScrollDirection = false;
            
            m_DefaultMonochromeColor = RNGNStaticData.DefaultMonochromeColor;
            m_DefaultMonochromeColorLight = RNGNStaticData.DefaultMonochromeColorLight;
            m_DefaultDrawerSettings.MonochromeColor = EditorGUIUtility.isProSkin ? m_DefaultMonochromeColor : m_DefaultMonochromeColorLight;
            
            m_TestColorGradient.SetKeys(RNGNStaticData.TestGradientColorKeys, RNGNStaticData.GradientAlphaKeys);
            m_TestColorGradientLight.SetKeys(RNGNStaticData.TestGradientColorKeysLight, RNGNStaticData.GradientAlphaKeys);
            
            m_SpreadColorGradient.SetKeys(RNGNStaticData.SpreadGradientColorKeys, RNGNStaticData.GradientAlphaKeys);
            m_SpreadColorGradientLight.SetKeys(RNGNStaticData.SpreadGradientColorKeysLight, RNGNStaticData.GradientAlphaKeys);

            m_EditorLogLevel = LogMessageType.Info | LogMessageType.Hint | LogMessageType.Warning;
            m_AllowLogColors = true;
            SetLoggerSettings();
            
            RLogger.Log("Default preferences loaded.", LogMessageType.Info);
        }

        private void SetLoggerSettings()
        {
            RNGNeedsCore.SetLogLevel(EditorLogLevel);
            RNGNeedsCore.SetLogAllowColors(m_AllowLogColors);
        }

        public void SetEditable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }

        public void Save()
        {
            Save(true);
        }

        public static void ResetColorPalettesToDefaults()
        {
            var confirm = EditorUtility.DisplayDialog(RNGNStaticData.ResetPalettesWindowTitle, RNGNStaticData.ResetPalettesWindowText, RNGNStaticData.ResetPalettesWindowConfirm, RNGNStaticData.ConfirmWindowCancel);
            if (confirm == false) return;
            instance.SetDefaultColorPalettes();
        }

        public static void ResetPreferencesToDefaults()
        {
            var confirm = EditorUtility.DisplayDialog(RNGNStaticData.ResetPreferencesWindowTitle, RNGNStaticData.ResetPreferencesWindowText, RNGNStaticData.ResetPreferencesWindowConfirm, RNGNStaticData.ConfirmWindowCancel);
            if (confirm == false) return;
            instance.SetGlobalDefaults();
        }
        
        private void SetDefaultColorPalettes()
        {
            var userPath = $"{RNGNStaticData.PathToEditorAssets}RNGNeedsPalettes.json";
            var loadedPalettes = JsonUtils.LoadObjectFromFile<ListWrapper<ProbabilityListColorPalette>>(userPath);
            if (loadedPalettes != null)
            {
                instance.colorPalettes = loadedPalettes.List;
                RLogger.Log("Default Palettes Loaded.", LogMessageType.Info);
            }
            else
            {
                RLogger.Log($"The default palettes file 'RNGNeedsPalettes.json' was not found in {userPath}.", LogMessageType.Warning);
                colorPalettes = new List<ProbabilityListColorPalette>
                {
                    new ProbabilityListColorPalette
                    {
                        colors = RNGNStaticData.DefaultPaletteColors,
                        palettePath = "Default"
                    }
                };
            }
        }

        public static void ImportPalettes()
        {
            var userPath = EditorUtility.OpenFilePanel("Import Palettes JSON", Application.dataPath, "json");
            if (string.IsNullOrEmpty(userPath)) return;
            var confirm = EditorUtility.DisplayDialog(RNGNStaticData.OverwritePalettesWindowTitle, RNGNStaticData.OverwritePalettesWindowText, RNGNStaticData.OverwritePalettesWindowConfirm, RNGNStaticData.ConfirmWindowCancel);
            if (confirm == false) return;

            var loadedPalettes = JsonUtils.LoadObjectFromFile<ListWrapper<ProbabilityListColorPalette>>(userPath);
            if (loadedPalettes != null)
            {
                instance.colorPalettes = loadedPalettes.List;
                RLogger.Log($"Color Palettes imported from {userPath}", LogMessageType.Info);
            }
            else
            {
                RLogger.Log("Color Palettes import failed", LogMessageType.Warning);
            }
        }
        
        [Serializable]
        private class ListWrapper<T>
        {
            public List<T> List;
        }
        
        public static void ExportPalettes()
        {
            var fileName = $"Saved Palettes {DateTime.Now:dd-MM-yyyy HH-mm}";
            var userPath = EditorUtility.SaveFilePanel("Export Palettes JSON", Application.dataPath, fileName, "json");
            if (string.IsNullOrEmpty(userPath)) return;

            var wrappedPalettes = new ListWrapper<ProbabilityListColorPalette>() { List = instance.colorPalettes };
            if (JsonUtils.SaveObjectToFile(wrappedPalettes, userPath, true))
            {
                AssetDatabase.Refresh();
                RLogger.Log($"Color Palettes exported to {userPath}", LogMessageType.Info);
            }
            else
            {
                RLogger.Log("Color Palettes export failed.", LogMessageType.Warning);
            }
        }
        
        public List<Color> GetColorsFromPalette(string _palettePath)
        {
            foreach (var palette in colorPalettes)
                if (palette.palettePath.Equals(_palettePath) && palette.colors.Count > 0)
                {
                    palette.SetDefaultAlpha();
                    return palette.colors;
                }
            return RNGNStaticData.DefaultPaletteColors;
        }

        public List<string> GetColorPalettePaths()
        {
            var palettePaths = new List<string>();
            foreach (var palette in colorPalettes) palettePaths.Add(palette.palettePath);
            return palettePaths;
        }
    }
}