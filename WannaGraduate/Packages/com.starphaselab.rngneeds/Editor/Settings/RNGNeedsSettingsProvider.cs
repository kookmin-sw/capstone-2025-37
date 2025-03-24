using System;
using System.Collections.Generic;
using System.Reflection;
using StarphaseTools.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RNGNeeds.Editor
{
    public class RNGNeedsSettingsProvider : SettingsProvider
    {
        private SerializedObject m_SerializedObject;
        private SerializedProperty p_DrawerOptionsLevel;
        private SerializedProperty p_DrawerOptionButtons;
        private SerializedProperty p_InspectorRefreshMode;
        
        private SerializedProperty p_InvertScrollDirection;
        
        private SerializedProperty p_DefaultMonochromeColor;
        private SerializedProperty p_TestColorGradient;
        private SerializedProperty p_SpreadColorGradient;
        
        private SerializedProperty p_DefaultDrawerSettings;
        private SerializedProperty p_colorPalettes;
        // private SerializedProperty p_DrawerSettings;
        private SerializedProperty p_EditorLogLevel;
        private SerializedProperty p_AllowLogColors;
        
        private readonly Color separatorColor = new Color(.5f, .6f, .7f);
        private const float titleBarButtonHeight = 20f;
        private const float buttonHeight = 30f;

        private class Styles
        {
            public static GUIContent DrawerOptionsLevel = new GUIContent(nameof(RNGNeedsSettings.DrawerOptionsLevel));
            public static GUIContent DrawerOptionButtons = new GUIContent(nameof(RNGNeedsSettings.DrawerOptionButtons));
            public static GUIContent InspectorRefreshMode = new GUIContent(nameof(RNGNeedsSettings.InspectorRefreshMode));
            
            public static GUIContent InvertScrollDirection = new GUIContent(nameof(RNGNeedsSettings.InvertScrollDirection));
            
            public static GUIContent DefaultMonochromeColor = new GUIContent("Default Monochrome Color");
            public static GUIContent TestColorGradient = new GUIContent(nameof(RNGNeedsSettings.TestColorGradient));
            public static GUIContent SpreadColorGradient = new GUIContent(nameof(RNGNeedsSettings.SpreadColorGradient));
            
            public static GUIContent DefaultDrawerSettings = new GUIContent("Default Drawer Settings");
            public static GUIContent ColorPalettes = new GUIContent(nameof(RNGNeedsSettings.colorPalettes));

            public static GUIContent EditorLogLevel = new GUIContent(nameof(RNGNeedsSettings.EditorLogLevel));
            public static GUIContent AllowLogColors = new GUIContent(nameof(RNGNeedsSettings.AllowLogColors));
        }

        private RNGNeedsSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            RNGNeedsSettings.instance.SetEditable();
            m_SerializedObject = new SerializedObject(RNGNeedsSettings.instance);

            p_DrawerOptionsLevel = m_SerializedObject.FindProperty("m_DrawerOptionsLevel");
            p_DrawerOptionButtons = m_SerializedObject.FindProperty("m_DrawerOptionButtons");
            p_InspectorRefreshMode = m_SerializedObject.FindProperty("m_InspectorRefreshMode");
            
            p_InvertScrollDirection = m_SerializedObject.FindProperty("m_InvertScrollDirection");
            
            p_DefaultMonochromeColor = m_SerializedObject.FindProperty(EditorGUIUtility.isProSkin ? "m_DefaultMonochromeColor" : "m_DefaultMonochromeColorLight");
            p_TestColorGradient = m_SerializedObject.FindProperty(EditorGUIUtility.isProSkin ? "m_TestColorGradient" : "m_TestColorGradientLight");
            p_SpreadColorGradient = m_SerializedObject.FindProperty(EditorGUIUtility.isProSkin ? "m_SpreadColorGradient" : "m_SpreadColorGradientLight");
            
            p_DefaultDrawerSettings = m_SerializedObject.FindProperty("m_DefaultDrawerSettings");
            p_colorPalettes = m_SerializedObject.FindProperty("colorPalettes");
            // p_DrawerSettings = m_SerializedObject.FindProperty("DrawerSettings");

            p_EditorLogLevel = m_SerializedObject.FindProperty("m_EditorLogLevel");
            p_AllowLogColors = m_SerializedObject.FindProperty("m_AllowLogColors");
            
            #if UNITY_2021_1_OR_NEWER
            var evt = Event.current;
            if (evt != null && (evt.modifiers & EventModifiers.Control) != 0 &&
                (evt.modifiers & EventModifiers.Shift) != 0)
            {
                RNGNeedsSettings.DevMode = !RNGNeedsSettings.DevMode;
                if (RNGNeedsSettings.DevMode)
                {
                    p_EditorLogLevel.enumValueFlag = (int)LogMessageType.All;
                    Save();
                    RLogger.Log("Dev Mode On", LogMessageType.Debug);
                }
                else
                {
                    RLogger.Log("Dev Mode Off", LogMessageType.Debug);
                    p_EditorLogLevel.enumValueFlag = (int)(LogMessageType.Info | LogMessageType.Hint);
                    Save();
                }
            }
            #endif
        }
        
        public override void OnGUI(string searchContext)
        {
            using (CreateSettingsWindowGUIScope())
            {
                m_SerializedObject.Update();

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(p_DrawerOptionsLevel);
                    EditorGUILayout.PropertyField(p_DrawerOptionButtons);
                    EditorGUILayout.PropertyField(p_InspectorRefreshMode);

                    EditorGUILayout.PropertyField(p_InvertScrollDirection);

                    EditorGUILayout.PropertyField(p_DefaultMonochromeColor);
                    EditorGUILayout.PropertyField(p_TestColorGradient);
                    EditorGUILayout.PropertyField(p_SpreadColorGradient);
                    if (check.changed) Save();
                }

                DrawSeparator();
                
                EditorGUILayout.LabelField("Logger Settings", EditorStyles.boldLabel);
                
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(p_EditorLogLevel);
                    if (check.changed)
                    {
                        Save();
                        RNGNeedsCore.SetLogLevel(RNGNeedsSettings.instance.EditorLogLevel);
                    }
                }

                
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(p_AllowLogColors);
                    if (check.changed)
                    {
                        Save();
                        RNGNeedsCore.SetLogAllowColors(RNGNeedsSettings.instance.AllowLogColors);
                        var tmpLevel = RNGNeedsSettings.instance.EditorLogLevel;
                        RNGNeedsCore.SetLogLevel(LogMessageType.All);
                        RLogger.Log("Preview Info Message", LogMessageType.Info);
                        RLogger.Log("Preview Hint Message", LogMessageType.Hint);
                        RLogger.Log("Preview Warning Message", LogMessageType.Warning);
                        RNGNeedsCore.SetLogLevel(tmpLevel);
                    }
                }

                // Welcome Window Button
                var welcomeWindowButtonRect = EditorGUILayout.GetControlRect(false, buttonHeight);
                GUI.BeginGroup(welcomeWindowButtonRect);
                if(GUI.Button(new Rect(0f, 0f, welcomeWindowButtonRect.width, buttonHeight), "Open RNGNeeds Welcome Window")) WelcomeWindow.ShowWindow();
                GUI.EndGroup();
                
                DrawSeparator();
                
                EditorGUILayout.LabelField("Drawer Options", EditorStyles.boldLabel);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(p_DefaultDrawerSettings);

                    DrawSeparator();

                    EditorGUILayout.LabelField("Palette Options", EditorStyles.boldLabel);

                    var controlRect = EditorGUILayout.GetControlRect(false, buttonHeight);
                    GUI.BeginGroup(controlRect);
                    var savePalettesButtonRect = new Rect(0f, 0f, controlRect.width * .3f, buttonHeight);
                    var loadPalettesButtonRect = new Rect(savePalettesButtonRect.xMax, 0f, controlRect.width * .3f, buttonHeight);
                    var resetPalettesButtonRect = new Rect(loadPalettesButtonRect.xMax, 0f, controlRect.width * .4f, buttonHeight);
                    if (GUI.Button(savePalettesButtonRect, "Export Palettes")) RNGNeedsSettings.ExportPalettes();
                    if (GUI.Button(loadPalettesButtonRect, "Import Palettes")) RNGNeedsSettings.ImportPalettes();
                    if (GUI.Button(resetPalettesButtonRect, "Reset to Defaults")) RNGNeedsSettings.ResetColorPalettesToDefaults();
                    GUI.EndGroup();

                    DrawSeparator();

                    EditorGUILayout.PropertyField(p_colorPalettes);

                    if (check.changed) Save();
                }
            }
        }

        private void Save()
        {
            m_SerializedObject.ApplyModifiedProperties();
            RNGNeedsSettings.instance.Save();
        }

        private void DrawSeparator()
        {
            var controlRect = EditorGUILayout.GetControlRect(true, 14f);
            GUI.BeginGroup(controlRect);
            EditorGUI.DrawRect(new Rect(0f, 12f, controlRect.xMax - 12f, 1f), separatorColor);
            GUI.EndGroup();
        }

        public override void OnTitleBarGUI()
        {
            // Main Separator
            var controlRect = EditorGUILayout.GetControlRect(true, 46f);
            var width = controlRect.xMax - 11f;
            EditorGUI.DrawRect(new Rect(10f, 45f, width, 1f), separatorColor);
            
            GUI.BeginGroup(controlRect);
            
            // Reset Preferences Button
            var buttonRect = new Rect(0f, 1f, controlRect.width, titleBarButtonHeight);
            if(GUI.Button(buttonRect, "Reset Preferences"))
            {
                RNGNeedsSettings.ResetPreferencesToDefaults();
                Save();
            }

            // Documentation Link
            var docsLinkRect = new Rect(0, buttonRect.yMax, controlRect.width, 20f);
            var docsLinkStyle = new GUIStyle(EditorStyles.linkLabel) { alignment = TextAnchor.MiddleCenter };
            EditorGUIUtility.AddCursorRect(docsLinkRect, MouseCursor.Link);
            
            if(GUI.Button(docsLinkRect, "Documentation / Manual", docsLinkStyle))
            {
                Application.OpenURL("https://docs.rngneeds.com/user-guide/introduction");
            }
            
            GUI.EndGroup();
        }
        
        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            var provider = new RNGNeedsSettingsProvider("Preferences/RNGNeeds", SettingsScope.User, GetSearchKeywordsFromGUIContentProperties<Styles>());
            return provider;
        }
        
        // https://www.listechblog.com/2022/02/use-settingsprovider-to-save-and-load-settings-in-unity-project-settings-or-preferences-window
        private static IDisposable CreateSettingsWindowGUIScope()
        {
            var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
            return Activator.CreateInstance(type) as IDisposable;
        }
    }
}