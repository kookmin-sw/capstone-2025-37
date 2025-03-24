using UnityEditor;

namespace RNGNeeds.Editor.Editors
{
    [InitializeOnLoad]
    public class RNGNeedsStartup
    {
        static RNGNeedsStartup()
        {
            if (EditorPrefs.GetBool("RNGNeeds_WelcomeWindow", false)) return;
            WelcomeWindow.ShowWindow();
            EditorPrefs.SetBool("RNGNeeds_WelcomeWindow", true);
        }
    }
}