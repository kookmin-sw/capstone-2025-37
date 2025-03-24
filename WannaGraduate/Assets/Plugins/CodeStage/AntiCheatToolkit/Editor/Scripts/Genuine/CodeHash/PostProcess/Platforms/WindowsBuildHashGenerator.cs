#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Genuine.CodeHash;

namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	internal static class WindowsBuildHashGenerator
	{
		public static async Task<IReadOnlyList<BuildHashes>> GetBuildHashes(string buildPath)
		{
			var folder = Path.GetDirectoryName(buildPath);
			if (folder == null)
			{
				ACTk.PrintExceptionForSupport("Could not found build folder for this file: " + buildPath);
				return Array.Empty<BuildHashes>();
			}
			
			var progress = FilesProgress.CreateNew("ACTk: Generating code hash");
			var filters = GetFilters();
			var buildHashes = await Task.Run(()=> StandaloneWindowsWorker.GetBuildHashes(folder, filters, Environment.ProcessorCount,
				progress));
			
			if (buildHashes == null)
				return Array.Empty<BuildHashes>();

			return new [] { buildHashes };
		}
		
		private static FilteringData GetFilters()
		{
			var il2Cpp = false;
#if UNITY_EDITOR
			il2Cpp = SettingsUtils.IsIL2CPPEnabled();
#elif ENABLE_IL2CPP
			il2Cpp = true;
#endif
			return FiltersProducer.GetFileFiltersStandaloneWindows(il2Cpp);
		}
	}
}