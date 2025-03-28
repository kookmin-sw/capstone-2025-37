#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Genuine.CodeHash;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	/// <summary>
	/// Does calculates code hash after build if you use option "Generate code hash".
	/// Listen to HashesGenerated or look for hash for each build in the Editor Console.
	/// </summary>
	/// Resulting hash in most cases should match value you get from the \ref CodeStage.AntiCheat.Genuine.CodeHash.CodeHashGenerator "CodeHashGenerator"
	public class CodeHashGeneratorPostprocessor : IPostprocessBuildWithReport
	{
		/// <summary>
		/// Equals int.MaxValue to make sure this postprocessor will run as late as possible
		/// so you could run own postprocessors before and subscribe to HashesGenerated event.
		/// </summary>
		/// Used at CodeHashGeneratorListener example.
		public const int CallbackOrder = int.MaxValue;
		
		/// <summary>
		/// HashesGenerated event delegate.
		/// </summary>
		/// <param name="report">Standard post-build report from Unity.</param>
		/// <param name="hashedBuilds">Build hashing results array.</param>
		///
		/// You may generate multiple actual builds within single build operation,
		/// like multiple APKs when you use "Split APKs by target architecture" option,
		/// so you may have more than one valid hashed builds for one actual build procedure.
		public delegate void OnHashesGenerated(BuildReport report, IReadOnlyList<BuildHashes> hashedBuilds);

		/// <summary>
		/// You may listen to this event if you wish to post-process resulting code hash,
		/// e.g. upload it to the server for the later runtime check with CodeHashGenerator.
		/// </summary>
		/// Make sure to enable "Generate code hash on build completion" option in the ACTk settings to make this event work.
		public static event OnHashesGenerated HashesGenerated;

		int IOrderedCallback.callbackOrder => CallbackOrder;
		async void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
		{
			if (!ACTkSettings.Instance.PreGenerateBuildHash || !CodeHashGenerator.IsTargetPlatformCompatible())
				return;
			
			await CalculateBuildReportHashesAsync(report, true, true);
		}
		
		/// <summary>
		/// Calls selection dialog and calculates hashes for the selected build.
		/// </summary>
		/// <param name="buildPath">Path to the .apk / .aab or .exe file. Pass null to show file selection dialog.</param>
		/// <param name="printToConsole">Path to the .apk / .aab or .exe file. Pass null to show file selection dialog.</param>
		/// <returns>Valid BuildHashes instance or null in case of error / user cancellation.</returns>
		public static BuildHashes CalculateExternalBuildHashes(string buildPath, bool printToConsole)
		{
			return AsyncHelpers.RunSync(() => CalculateExternalBuildHashesAsync(buildPath, printToConsole));
		}
		
		/// <summary>
		/// Calls selection dialog and calculates hashes for the selected build.
		/// </summary>
		/// <param name="buildPath">Path to the .apk / .aab or .exe file. Pass null to show file selection dialog.</param>
		/// <param name="printToConsole">Path to the .apk / .aab or .exe file. Pass null to show file selection dialog.</param>
		/// <returns>Task with Valid BuildHashes instance or null in case of error / user cancellation.</returns>
		public static async Task<BuildHashes> CalculateExternalBuildHashesAsync(string buildPath, bool printToConsole)
		{
			if (buildPath == null)
			{
				buildPath = EditorUtility.OpenFilePanel(
					"Select Standalone Windows build exe or Android build apk / aab", "", "exe,apk,aab");
				if (string.IsNullOrEmpty(buildPath))
				{
					Debug.Log(ACTk.LogPrefix + "Hashing cancelled by user.");
					return null;
				}
			}

			var extension = Path.GetExtension(buildPath);
			if (string.IsNullOrEmpty(extension))
				return null;
			
			extension = extension.ToLower(CultureInfo.InvariantCulture);

			try
			{
				EditorUtility.DisplayProgressBar("ACTk: Generating code hash", "Preparing...", 0);
				BuildHashes hashes;
				
				if (extension == ".apk" || extension == ".aab")
					hashes = (await AndroidBuildHashGenerator.GetBuildHashes(new[] { buildPath })).FirstOrDefault();
				else
					hashes = (await WindowsBuildHashGenerator.GetBuildHashes(buildPath)).FirstOrDefault();
				
				if (printToConsole)
					hashes?.PrintToConsole();

				return hashes;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Error while trying to hash build!", e);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			return null;
		}
		
		/// <summary>
		/// Calculates hashes for the given build report. Can be useful if you wish to hash build you just made with BuildPipeline.
		/// </summary>
		/// Will not trigger the HashesGenerated event.
		/// <param name="report">BuildReport you wish to calculates hashes for</param>
		/// <param name="printToConsole">Specifies if calculated hashes should be printed to Unity Console</param>
		/// <returns>Readonly List of the BuildHashes, one per each resulting build from the BuildReport.</returns>
		public static Task<IReadOnlyList<BuildHashes>> CalculateBuildReportHashesAsync(BuildReport report,
			bool printToConsole)
		{
			return CalculateBuildReportHashesAsync(report, false, printToConsole);
		}

		private static async Task<IReadOnlyList<BuildHashes>> CalculateBuildReportHashesAsync(BuildReport report,
			bool triggerEvent, bool printLogs)
		{
			if (EditorUserBuildSettings.GetPlatformSettings(report.summary.platformGroup.ToString(),
					"CreateSolution") == "true")
			{
				Debug.Log(
					ACTk.LogPrefix + "Build hashing is skipped due to the 'Create Visual Studio Solution' option.");
				return null;
			}
			
			try
			{
				EditorUtility.DisplayProgressBar("ACTk: Generating code hash", "Preparing...", 0);
				
				var hashedBuilds = await GetHashedBuilds(report);

				if (hashedBuilds == null || hashedBuilds.Count == 0)
				{
					Debug.Log(ACTk.LogPrefix + "Couldn't pre-generate code hash. " +
							  "Please run your build and generate hash with CodeHashGenerator.");
					return null;
				}

				if (printLogs)
				{
					foreach (var hashedBuild in hashedBuilds)
					{
						hashedBuild.PrintToConsole();
					}
				}
				
				if (triggerEvent)
					HashesGenerated?.Invoke(report, hashedBuilds);

				return hashedBuilds;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Error while trying to hash build!", e);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
			
			return null;
		}

		private static async Task<IReadOnlyList<BuildHashes>> GetHashedBuilds(BuildReport report)
		{
			var platform = report.summary.platform;
			switch (platform)
			{
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneWindows:
					return await WindowsBuildHashGenerator.GetBuildHashes(report.summary.outputPath);
				case BuildTarget.Android:
					return await AndroidBuildHashGenerator.GetBuildHashes(report);
				default:
					return null;
			}
		}
	}
}