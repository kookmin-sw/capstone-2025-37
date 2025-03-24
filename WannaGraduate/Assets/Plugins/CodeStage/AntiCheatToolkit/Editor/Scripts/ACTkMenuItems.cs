#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.ObscuredTypes;

namespace CodeStage.AntiCheat.EditorCode
{
	using Detectors;
	using UnityEditor;

	using Common;
	using PostProcessors;
	using UnityEngine;
	using Processors;

	internal static class ACTkMenuItems
	{
		// ---------------------------------------------------------------
		//  Main menu items
		// ---------------------------------------------------------------

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Settings...", false, 100)]
		private static void ShowSettingsWindow()
		{
			ACTkSettings.Show();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Injection Detector Whitelist Editor...", false, 1000)]
		private static void ShowAssembliesWhitelistWindow()
		{
			UserWhitelistEditor.ShowWindow();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Calculate external build hashes", false, 1200)]
		private static async void HashExternalBuild()
		{
			var buildHashes = await CodeHashGeneratorPostprocessor.CalculateExternalBuildHashesAsync(null, true);
			if (buildHashes == null || buildHashes.FileHashes.Count == 0)
			{
				Debug.LogError(ACTk.LogPrefix + "External build hashing was not successful. " +
				               "See previous log messages for possible details.");
			}
		}
		
		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Configure proguard-user.txt", false, 1201)]
		private static void CheckProGuard()
		{
			BuildPreProcessor.CheckProGuard(true);
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate obscured types in assets...", false, 1500)]
		private static void MigrateObscuredTypesInAssets()
		{
			ObscuredTypesMigrator.MigrateProjectAssets();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate obscured types in opened scene(s)...", false, 1501)]
		private static void MigrateObscuredTypesInScene()
		{
			ObscuredTypesMigrator.MigrateOpenedScenes();
		}		
		
		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Validate/Validate obscured types in assets...", false, 1500)]
		private static void ValidateObscuredTypesInAssets()
		{
			var invalidPropertiesFound = ObscuredTypesValidator.ValidateProjectAssets();
			if (invalidPropertiesFound > 0)
			{
				var result = ConfirmMigrationAfterValidation();
				
				switch (result)
				{
					case 0:
						ObscuredTypesMigrator.MigrateProjectAssets(true);
						break;
					case 2:
						ObscuredTypesMigrator.MigrateProjectAssets(true, true);
						break;
				}
			}
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Validate/Validate obscured types in opened scene(s)...", false, 1501)]
		private static void ValidateObscuredTypesInOpenedScenes()
		{
			var invalidPropertiesFound = ObscuredTypesValidator.ValidateOpenedScenes();
			if (invalidPropertiesFound > 0)
			{
				var result = ConfirmMigrationAfterValidation();
				switch (result)
				{
					case 0:
						ObscuredTypesMigrator.MigrateOpenedScenes(true);
						break;
					case 2:
						ObscuredTypesMigrator.MigrateOpenedScenes(true, true);
						break;
				}
			}
		}
		
		private static int ConfirmMigrationAfterValidation()
		{
			return EditorUtility.DisplayDialogComplex(ObscuredTypesValidator.ModuleName,
				"Invalid types found. It's recommended to try migrate and / or fix them.\n" +
				"You may need to migrate only if you did update from the older ACTk version.",
				"Migrate and fix", "Cancel", "Fix only");
		}

		// ---------------------------------------------------------------
		//  GameObject menu items
		// ---------------------------------------------------------------

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + "All detectors", false, 0)]
		private static void AddAllDetectorsToScene()
		{
			AddInjectionDetectorToScene();
			AddObscuredCheatingDetectorToScene();
			AddSpeedHackDetectorToScene();
			AddWallHackDetectorToScene();
			AddTimeCheatingDetectorToScene();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + InjectionDetector.ComponentName, false, 1)]
		private static void AddInjectionDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<InjectionDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + ObscuredCheatingDetector.ComponentName, false, 1)]
		private static void AddObscuredCheatingDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<ObscuredCheatingDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + SpeedHackDetector.ComponentName, false, 1)]
		private static void AddSpeedHackDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<SpeedHackDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + WallHackDetector.ComponentName, false, 1)]
		private static void AddWallHackDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<WallHackDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + TimeCheatingDetector.ComponentName, false, 1)]
		private static void AddTimeCheatingDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<TimeCheatingDetector>();
		}
	}
}