using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ProjectInitializer : Editor
{
    [MenuItem("Domino/InitDirectory")]
    public static void CreateDefaultFolders()
    {
        string[] defaultPaths = new string[]
        {
            "Scripts",
            "Prefabs",
            "Sprites",
            "Resources",
            "Sounds",
            "Animations",
            "Scripts/Game",
            "Scripts/Editor",
        };

        foreach(string path in defaultPaths)
        {
            string fullPath = Path.Combine(Application.dataPath, path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Completed", "Directories are initialized", "OK");
    }
}

public class UnityPackageBatchmporter : EditorWindow
{
    string importPath => Application.dataPath + "/DominoGames/Dependencies";

    [MenuItem("Domino/Import All UnityPackages")]
    public static void ShowWindow()
    {
        GetWindow<UnityPackageBatchmporter>("Batch Import UnityPackages");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import All UnityPackages", EditorStyles.boldLabel);
        GUILayout.Space(10);
        if(GUILayout.Button("Import all unity packages", GUILayout.Height(30))){
            if(!string.IsNullOrEmpty(importPath) && Directory.Exists(importPath))
            {
                ImportUnityPackages(importPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a valid folder.", "OK");
            }
        }
    }

    private void ImportUnityPackages(string path)
    {
        string[] packageFiles = Directory.GetFiles(path, "*.unitypackage", SearchOption.AllDirectories);

        if(packageFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("No packages found", "No .unitypackage files were found!", "OK");
            return;
        }

        EditorApplication.LockReloadAssemblies();

        foreach(string packageFile in packageFiles)
        {
            AssetDatabase.ImportPackage(packageFile, false);
            Debug.Log($"Imported: {packageFile}");
        }

        EditorApplication.UnlockReloadAssemblies();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Import Complete", $"{packageFiles.Length} UnityPackages are imported", "OK");
    }
}
