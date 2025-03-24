#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using System.Threading.Tasks;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Utils;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal class StandaloneWindowsWorker : BaseWorker
	{
		private static readonly object ProgressLock = new object();
		
		public static BuildHashes GetBuildHashes(string buildPath, FilteringData filters, int numCores, 
			IProgress<FilesProgress> progress = null)
		{
			var sw = Stopwatch.StartNew();

			var files = Directory.GetFiles(buildPath, "*", SearchOption.AllDirectories);
			var count = files.Length;
			if (count == 0)
				return null;
			
			var fileHashes = new ConcurrentBag<FileHash>();
			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = numCores,
			};

			var latestPercent = 0;
			var filesChecked = 0;

			var sha1Pool = new ThreadSafeDisposablesPool<SHA1Wrapper>(() => new SHA1Wrapper());
			
			Parallel.ForEach(files, options, (filePath, state) =>
			{
				var skipped = true;
				try
				{
					// skip folders since we can't hash them
					if (Directory.Exists(filePath))
						return;

					if (filters.IsIgnored(filePath))
						return;

					if (!filters.IsIncluded(filePath))
						return;
					
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					using (var bs = new BufferedStream(fs))
					{
						//Debug.Log("Path: " + filePath + "\nHash: " + hashString);
						fileHashes.Add(new FileHash(filePath, bs, sha1Pool));
					}

					skipped = false;
				}
				catch (Exception e)
				{
					ACTk.PrintExceptionForSupport($"Something went wrong while calculating {filePath} hash in {buildPath}!", e);
				}
				finally
				{
					lock (ProgressLock)
					{
						filesChecked++;
						if (!skipped)
							progress?.ReportPercent(ref latestPercent, filesChecked, count, Path.GetFileName(filePath));
					}
				}
			});

			sha1Pool.Dispose();
			progress?.Report(FilesProgress.None());
			sw.Stop();

			if (fileHashes.Count == 0)
				return null;

			var result = new BuildHashes(buildPath, fileHashes.ToArray())
			{
				DurationSeconds = sw.Elapsed.TotalSeconds
			};

			return result;
		}

		public StandaloneWindowsWorker(int threadsCount) : base(threadsCount) { }

		public override void Execute()
		{
			base.Execute();

			try
			{
				var buildFolder = Path.GetFullPath(Application.dataPath + @"\..\");
#if ENABLE_IL2CPP
				var il2cpp = true;
#else
				var il2cpp = false;
#endif
				var filters = FiltersProducer.GetFileFiltersStandaloneWindows(il2cpp);
				Task.Run(()=>GenerateHashThread(buildFolder, filters));
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Something went wrong while calculating hash!", e);
				Complete(HashGeneratorResult.FromError(e.ToString()));
			}
		}

		private void GenerateHashThread(string folder, FilteringData filteringData)
		{
			try
			{
				var buildHashes = GetBuildHashes(folder, filteringData, threadsCount);
				Complete(HashGeneratorResult.FromBuildHashes(buildHashes));
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport("Something went wrong in hashing thread!", e);
				Complete(HashGeneratorResult.FromError(e.ToString()));
			}
		}
	}
}