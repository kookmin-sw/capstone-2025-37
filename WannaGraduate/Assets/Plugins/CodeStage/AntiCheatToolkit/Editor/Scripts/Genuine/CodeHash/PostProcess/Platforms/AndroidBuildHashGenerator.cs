#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.EditorCode.ICSharpCode.SharpZipLib.Zip;
using CodeStage.AntiCheat.Genuine.CodeHash;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	internal static class AndroidBuildHashGenerator
	{
		private static readonly object ProgressLock = new object();

		public static async Task<BuildHashes[]> GetBuildHashes(BuildReport report)
		{
			if ((report.summary.options & BuildOptions.PatchPackage) != 0)
			{
				Debug.Log(ACTk.LogPrefix + "Patch hashing is skipped, only full build hashing is supported.");
				return Array.Empty<BuildHashes>();
			}

#if UNITY_2022_1_OR_NEWER
			var files = report.GetFiles();
#else
			var files = report.files;
#endif
			var filePaths = new string[files.Length];
			for (var i = 0; i < filePaths.Length; i++)
			{
				filePaths[i] = files[i].path;
			}
			
			return await GetBuildHashes(filePaths);
		}

		public static async Task<BuildHashes[]> GetBuildHashes(string[] files)
		{
			var result = new List<BuildHashes>();
			
			foreach (var path in files)
			{
				var extension = Path.GetExtension(path);
				if (!string.IsNullOrEmpty(extension))
					extension = extension.ToLower(CultureInfo.InvariantCulture);

				if (extension != ".apk" && extension != ".aab")
					continue;

				var progress = FilesProgress.CreateNew("ACTk: Generating code hash");
				var filters = GetFilters();
				var buildHashes = await Task.Run(() => HashSuitableFilesInZipFile(path, filters,
					progress));
				if (buildHashes != null)
					result.Add(buildHashes);
			}

			if (result.Count == 0)
			{
				if (!EditorUserBuildSettings.exportAsGoogleAndroidProject)
					ACTk.PrintExceptionForSupport("Couldn't find compiled APK or AAB build!");
				else
					Debug.LogWarning("Couldn't find compiled APK or AAB build! " +
									 "This is fine if you use Export Project feature.");
			}
			
			return result.ToArray();
		}

		private static BuildHashes HashSuitableFilesInZipFile(string path, FilteringData filters,
			IProgress<FilesProgress> progress)
		{
			ZipFile zf = null;

			try
			{
				var sw = Stopwatch.StartNew();

				var latestPercent = 0;
				var filesChecked = 0;
				var fileHashes = new ConcurrentBag<FileHash>();
				var fs = File.OpenRead(path);
				zf = new ZipFile(fs);
				var count = zf.Count;

				var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
				var sha1Pool = new ThreadSafeDisposablesPool<SHA1Wrapper>(() => new SHA1Wrapper());

				Parallel.ForEach(zf.Cast<ZipEntry>(), options, zipEntry =>
				{
					string entryFileName = null;
					var skipped = true;
					try
					{
						// skip folders since we can't hash them
						if (!zipEntry.IsFile)
							return;

						entryFileName = zipEntry.Name;
						if (filters.IsIgnored(entryFileName))
							return;

						if (!filters.IsIncluded(entryFileName))
							return;
						
						using (var zipStream = zf.GetInputStream(zipEntry))
						{
							fileHashes.Add(new FileHash(entryFileName, zipStream, sha1Pool));
						}

						skipped = false;
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}
					finally
					{
						lock (ProgressLock)
						{
							filesChecked++;
							if (!skipped)
								progress?.ReportPercent(ref latestPercent, filesChecked, count, Path.GetFileName(entryFileName));
						}
					}
				});

				sha1Pool.Dispose();
				progress?.Report(FilesProgress.None());
				sw.Stop();
				
				if (fileHashes.Count == 0)
					return null;

				var result = new BuildHashes(path, fileHashes.ToArray())
				{
					DurationSeconds = sw.Elapsed.TotalSeconds
				};

				return result;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Error while calculating code hash!", e);
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true;
					zf.Close();
				}
			}

			return null;
		}
		
		private static FilteringData GetFilters()
		{
			var il2Cpp = false;
#if UNITY_EDITOR
			il2Cpp = SettingsUtils.IsIL2CPPEnabled();
#elif ENABLE_IL2CPP
			il2Cpp = true;
#endif
			return FiltersProducer.GetFileFiltersAndroid(il2Cpp);
		}
	}
}