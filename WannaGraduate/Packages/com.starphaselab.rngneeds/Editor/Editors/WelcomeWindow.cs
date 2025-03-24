using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    public class WelcomeWindow : EditorWindow
    {
        private static readonly Vector2 windowSize = new Vector2(340f, 460f);
        private GUIStyle leadStyle;
        private GUIStyle headingStyle;
        private GUIStyle versionStyle;
        private GUIStyle docsLinkStyle;
        private GUIStyle paragraphStyle;
        
        private readonly Color separatorColor = new Color(.5f, .6f, .7f);
        private Texture2D rngneedsIcon;
        private string version;
        private bool Initialized;
        private string[] RandomNeeds;
        private string MysteryButtonText;
        private GUIStyle MysteryButtonStyle;
        
        private void Initialize()
        {
            leadStyle = new GUIStyle(GUI.skin.label) { wordWrap = true, fontStyle = FontStyle.Bold };
            headingStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold};
            versionStyle = new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter };
            docsLinkStyle = new GUIStyle(EditorStyles.linkLabel) { alignment = TextAnchor.MiddleCenter };
            paragraphStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true};
            version = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.starphaselab.rngneeds/package.json").version;
            rngneedsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.starphaselab.rngneeds/Editor/Assets/RNGNeeds_Icon.png");
            
            var fileContent = System.IO.File.ReadAllText("Packages/com.starphaselab.rngneeds/Editor/Assets/RandomNeeds.txt");
            var decryptedString = EncryptDecrypt(fileContent, 1234);
            var deserializedWrapper = JsonUtility.FromJson<StringWrapper>(decryptedString);
            RandomNeeds = deserializedWrapper.strings;
            MysteryButtonText = "Get inspired by a random need ...";
            
            MysteryButtonStyle = new GUIStyle
            {
                normal =
                {
                    background = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.starphaselab.rngneeds/Editor/Assets/PL_InputField_00000.png"),
                    textColor = Color.white
                },
                active =
                {
                    background = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.starphaselab.rngneeds/Editor/Assets/PL_SectionButtonOn_00000.png"),
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                wordWrap = true,
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(8, 8, 8, 8)
            };

            Initialized = true;
        }

        public static void ShowWindow()
        {
            WelcomeWindow window = (WelcomeWindow)GetWindow(typeof(WelcomeWindow), true, "Welcome to RNGNeeds!");
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.Show();
        }

        private void OnGUI()
        {
            if (Initialized == false) Initialize();
            
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    GUILayout.Label(rngneedsIcon, GUILayout.Width(64f), GUILayout.Height(64f));
                    GUILayout.Label($"RNGNeeds\nv{version}", versionStyle);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                    GUILayout.Label("Thank you for installing RNGNeeds.", leadStyle);
                    GUILayout.Label("Welcome to the realm of intuitive probability distribution. We're excited to have you on board and wish you the highest odds of an exceptional RNG journey!", paragraphStyle);
                    if (GUILayout.Button("Open RNGNeeds Preferences"))
                    {
                        SettingsService.OpenUserPreferences("Preferences/RNGNeeds");
                    }
                EditorGUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            DrawSeparator();
            GUILayout.Label("Useful Links", headingStyle);
            if(GUILayout.Button("Documentation / Manual", docsLinkStyle))
            {
                Application.OpenURL("https://docs.rngneeds.com/user-guide/introduction");
            }
            
            if(GUILayout.Button("Quick Start Guide", docsLinkStyle))
            {
                Application.OpenURL("https://docs.rngneeds.com/user-guide/getting-started#quick-start");
            }
            
            if(GUILayout.Button("Samples Overview", docsLinkStyle))
            {
                Application.OpenURL("https://docs.rngneeds.com/examples/examples-overview#exploring-the-samples");
            }
            
            DrawSeparator();
            GUILayout.Label("Importing Samples", headingStyle);
            GUILayout.Label("To import samples, navigate to the RNGNeeds package in the Package Manager (click the button below)", paragraphStyle);
            if (GUILayout.Button("Open RNGNeeds in Package Manager"))
            {
                UnityEditor.PackageManager.UI.Window.Open("com.starphaselab.rngneeds");
            }
            
            GUILayout.Label("Once there, find the 'Samples' section. From the list, simply click the 'Import' button adjacent to the samples you'd like to add to your project.", paragraphStyle);
            
            GUILayout.Label("Detailed step-by-step instructions:", paragraphStyle);
            if(GUILayout.Button("Importing Samples Guide", docsLinkStyle))
            {
                Application.OpenURL("https://docs.rngneeds.com/examples/examples-overview#importing-samples");
            }
            
            DrawSeparator();
            
            var controlRect = EditorGUILayout.GetControlRect(false, 64f);
            GUI.BeginGroup(controlRect);
            var mysteryButtonRect = new Rect(0f, 0f, controlRect.width, 64f);
            if (GUI.Button(mysteryButtonRect, MysteryButtonText, MysteryButtonStyle)) MysteryButtonText = RandomNeeds[Random.Range(0, RandomNeeds.Length)];
            GUI.EndGroup();
        }
        
        private void DrawSeparator()
        {
            var controlRect = EditorGUILayout.GetControlRect(true, 10f);
            GUI.BeginGroup(controlRect);
            EditorGUI.DrawRect(new Rect(0f, 4f, controlRect.xMax, 1f), separatorColor);
            GUI.EndGroup();
        }

        private static string EncryptDecrypt(string text, int key)
        {
            var chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(chars[i] ^ key);
            }
            return new string(chars);
        }
    }
}