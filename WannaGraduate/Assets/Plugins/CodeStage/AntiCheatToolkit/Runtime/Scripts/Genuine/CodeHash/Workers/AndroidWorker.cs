#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_ANDROID
#define ACTK_ANDROID_DEVICE
#endif

#if ACTK_ANDROID_DEVICE

using CodeStage.AntiCheat.Common;
using System;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal class AndroidWorker : BaseWorker
	{
		private Stopwatch sw;

		[System.Reflection.Obfuscation(Exclude = true, ApplyToMembers=false)]
		private class CodeHashGeneratorCallback : AndroidJavaProxy
		{
			private readonly AndroidWorker parent;

			public CodeHashGeneratorCallback(AndroidWorker parent) : base("net.codestage.actk.androidnative.CodeHashCallback")
			{
				this.parent = parent;
			}

			[System.Reflection.Obfuscation(Exclude = true)]
			// called from native Android plugin, from separate thread
			public void OnSuccess(string buildPath, string[] paths, string[] hashes, string summaryHash)
			{
				var fileHashes = new FileHash[hashes.Length];
				for (var i = 0; i < hashes.Length; i++)
				{
					var hash = hashes[i];
					var path = paths[i];

					fileHashes[i] = new FileHash(path, hash);
				}

				var buildHashes = new BuildHashes(buildPath, fileHashes, summaryHash);
				parent.sw.Stop();
				buildHashes.DurationSeconds = parent.sw.Elapsed.TotalSeconds;
				parent.Complete(HashGeneratorResult.FromBuildHashes(buildHashes));
			}

			[System.Reflection.Obfuscation(Exclude = true)]
			// called from native Android plugin, from separate thread
			public void OnError(string errorMessage)
			{
				parent.Complete(HashGeneratorResult.FromError(errorMessage));
			}
		}
		
		public AndroidWorker(int threadsCount) : base(threadsCount) { }

		public override void Execute()
		{
			base.Execute();

		    const string classPath = "net.codestage.actk.androidnative.CodeHashGenerator";

		    try
		    {
			    sw = Stopwatch.StartNew();
			    using (var nativeClass = new AndroidJavaClass(classPath))
			    {
#if ENABLE_IL2CPP
					var il2cpp = true;
#else
				    var il2cpp = false;
#endif

				    var filters = FiltersProducer.GetFileFiltersAndroid(il2cpp);
					nativeClass.CallStatic("GetCodeHash", GenerateStringArrayFromFilters(filters), new CodeHashGeneratorCallback(this), threadsCount);
			    }
		    }
		    catch (Exception e)
		    {
				ACTk.PrintExceptionForSupport("Can't initialize NativeRoutines!", e);
		    }
		}

		private string[] GenerateStringArrayFromFilters(FilteringData allFilters)
		{
			var serializedFilters = new List<string>();
			if (allFilters.Includes != null && allFilters.Includes.Length > 0)
				serializedFilters.AddRange(SerializeArray(allFilters.Includes));
			if (allFilters.Ignores != null && allFilters.Ignores.Length > 0)
			{
				serializedFilters.Add("IGNORES");
				serializedFilters.AddRange(SerializeArray(allFilters.Ignores));
			}
			return serializedFilters.ToArray();

			IEnumerable<string> SerializeArray(IList<FileFilter> filters)
			{
				if (filters == null || filters.Count == 0)
					return Array.Empty<string>();
				
				var itemsCount = filters.Count;
				var result = new string[itemsCount];
				for (var i = 0; i < itemsCount; i++)
				{
					result[i] = filters[i].ToString();
				}

				return result;
			}
		}
	}
}

#endif