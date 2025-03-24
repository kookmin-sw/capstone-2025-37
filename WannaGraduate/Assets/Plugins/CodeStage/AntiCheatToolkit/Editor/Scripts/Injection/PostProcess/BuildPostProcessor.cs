#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	using System;
	using System.IO;
	using System.Text;
	using Common;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;

	internal class BuildPostProcessor : IPreprocessBuildWithReport, IPostBuildPlayerScriptDLLs, IPostprocessBuildWithReport
	{
		int IOrderedCallback.callbackOrder => int.MaxValue - 1;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (!ACTkSettings.Instance.InjectionDetectorEnabled ||
			    !InjectionRoutines.IsInjectionPossible())
			{
				return;
			}

			InjectionRoutines.InitCleanup();
			Prepare();
		}

		public void OnPostBuildPlayerScriptDLLs(BuildReport report)
		{
			if (!ACTkSettings.Instance.InjectionDetectorEnabled ||
			    !InjectionRoutines.IsInjectionPossible())
			{
				return;
			}

			InjectionWhitelistBuilder.GenerateWhitelist();
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			Cleanup();
		}

		public static string[] GetGuessedLibrariesForBuild()
		{
			var stagingAreaFolder = Path.Combine(ACTkEditorConstants.ProjectTempFolder, "StagingArea");
			return EditorTools.FindLibrariesAt(stagingAreaFolder);
		}

		private void Prepare()
		{
			try
			{
				EditorApplication.LockReloadAssemblies();

				if (!Directory.Exists(InjectionConstants.ResourcesFolder))
				{
					Directory.CreateDirectory(InjectionConstants.ResourcesFolder);
				}

				File.WriteAllText(InjectionConstants.DataFilePath, "please remove me", Encoding.Unicode);
				AssetDatabase.Refresh();

				EditorApplication.update += OnEditorUpdate;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Injection Detector preparation failed!", e);
			}
			finally
			{
				EditorApplication.UnlockReloadAssemblies();
			}
		}
		
		private void OnEditorUpdate()
		{
			if (!BuildPipeline.isBuildingPlayer)
				Cleanup();
		}
		
		private void Cleanup()
		{
			InjectionRoutines.Cleanup();
			EditorApplication.update -= OnEditorUpdate;
		}
	}
}