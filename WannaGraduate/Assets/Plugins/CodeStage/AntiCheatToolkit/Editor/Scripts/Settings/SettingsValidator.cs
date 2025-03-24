#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.Validation
{
	using Detectors;
	using UnityEditor;

	[InitializeOnLoad]
	internal static class SettingsValidator
	{
		private static bool injectionValidated;
		private static bool wallhackValidated;

		static SettingsValidator()
		{
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
		}

		private static void OnHierarchyChanged()
		{
			if (!injectionValidated && !ACTkSettings.Instance.DisableInjectionDetectorValidation)
			{
				var instance = InjectionDetector.Instance;
				if (instance != null)
				{
					if (InjectionRoutines.IsInjectionPossible())
					{
						if (!ACTkSettings.Instance.InjectionDetectorEnabled)
						{
							var result = EditorUtility.DisplayDialogComplex("Anti-Cheat Toolkit Validation",
								"ACTk noticed you're using Injection Detector while having build injection detection support disabled.\n" +
								"It should be enabled in order to work properly.\nWould you like to enable it now?",
								"Yes", "Open Settings", "No, never ask again");

							switch (result)
							{
								case 0:
									ACTkSettings.Instance.InjectionDetectorEnabled = true;
									break;
								case 1:
									ACTkSettings.Show();
									injectionValidated = true;
									return;
								default:
									ACTkSettings.Instance.DisableInjectionDetectorValidation = true;
									break;
							}
						}
					}
				}
				injectionValidated = true;
			}

			if (!wallhackValidated && 
				(!ACTkSettings.Instance.DisableWallhackDetectorShaderValidation ||
				!ACTkSettings.Instance.DisableWallhackDetectorLinkXmlValidation ||
				!ACTkSettings.Instance.DisableWallhackDetectorPhysicsValidation))
			{
				var instance = WallHackDetector.Instance;
				if (instance != null)
				{
					if (!ACTkSettings.Instance.DisableWallhackDetectorShaderValidation && 
						instance.CheckWireframe && 
						!SettingsGUI.IsWallhackDetectorShaderIncluded())
					{
						var result = EditorUtility.DisplayDialog("Anti-Cheat Toolkit Validation",
							"ACTk noticed you're using WallHack Detector with Wireframe option enabled but you have no required shader added" +
							" to the Always Included Shaders.\n" +
							"Would you like to exit Play Mode and open Settings to include it now?",
							"Yes", "No, never ask again");

						if (result)
						{
							EditorApplication.isPlaying = false;
							ACTkEditorPrefsSettings.FocusWallhackFoldout();
							ACTkSettings.Show();
							wallhackValidated = true;
							return;
						}
						
						ACTkSettings.Instance.DisableWallhackDetectorShaderValidation = true;
					}

					if (!ACTkSettings.Instance.DisableWallhackDetectorPhysicsValidation &&
						(instance.CheckRigidbody || instance.CheckController) &&
						SettingsUtils.IsPhysicsUsesTemporalSolver())
					{
						var result = EditorUtility.DisplayDialog("Anti-Cheat Toolkit Validation",
							"ACTk noticed you're using WallHack Detector with either Rigidbody or Character Controller option enabled while having Project Settings > Physics > Solver Type property set to the Temporal Gauss Seidel value.\n\n" +
							"This can lead to the physics anomalies and unpredicted behavior due to how this solver operates while having few continuously colliding objects in the scene (which is going to happen when you use the WallHack Detector with current options).\n\n" +
							"It's recommended to switch the Solver Type to the Projected Gauss Seidel in such case.\n" +
							"Would you like to exit Play Mode and open Settings to switch it now?",
							"Yes", "No, never ask again");

						if (result)
						{
							EditorApplication.isPlaying = false;
							SettingsUtils.ShowSolverTypesProperty();
							wallhackValidated = true;
							return;
						}
						
						ACTkSettings.Instance.DisableWallhackDetectorPhysicsValidation = true;
					}

					if (!ACTkSettings.Instance.DisableWallhackDetectorLinkXmlValidation &&
						SettingsUtils.IsLinkXmlRequired() && 
						!SettingsUtils.IsLinkXmlEnabled())
					{
						var result = EditorUtility.DisplayDialog("Anti-Cheat Toolkit Validation",
							"ACTk noticed you're using WallHack Detector while having IL2CPP's " +
							"Strip Engine Code setting enabled which can lead to stripping of components " +
							"required by WallHack Detector causing false positives.\n\n" +
							"To prevent such stripping, components should be added to the link.xml so linker could exclude them from stripping.\n" +
							"Would you like to exit Play Mode and open Settings to enable automatic link.xml generation?",
							"Yes", "No, never ask again");

						if (result)
						{
							EditorApplication.isPlaying = false;
							ACTkEditorPrefsSettings.FocusWallhackFoldout();
							ACTkSettings.Show();
							wallhackValidated = true;
							return;
						}
						
						ACTkSettings.Instance.DisableWallhackDetectorLinkXmlValidation = true;
					}
				}
				wallhackValidated = true;
			}
		}
	}
}