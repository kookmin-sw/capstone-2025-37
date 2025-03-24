#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CodeStage.AntiCheat.Common;
using UnityEngine;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	/// <summary>
	/// Generates current application runtime code hash to let you validate it against previously generated runtime code hash to detect external code manipulations.
	/// </summary>
	/// Calculation is done on the separate threads where possible to prevent noticeable CPU spikes and performance impact.<br/>
	/// Supported platforms: Windows PC, Android (more to come)<br/>
	/// Resulting hash in most cases should match value you get from the \ref CodeStage.AntiCheat.EditorCode.PostProcessors.CodeHashGeneratorPostprocessor "CodeHashGeneratorPostprocessor".
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	public class CodeHashGenerator : KeepAliveBehaviour<CodeHashGenerator>, ICodeHashGenerator
	{
		private const int CompletionCheckIntervalMilliseconds = 500;
		
		/// <summary>
		/// Subscribe to get resulting hash right after it gets calculated.
		/// </summary>
		public static event HashGeneratorResultHandler HashGenerated;

		/// <summary>
		/// Stores previously calculated result.
		/// Can be null if Generate() wasn't called yet or if it was called but calculation is still in process.
		/// </summary>
		/// \sa #IsBusy
		public HashGeneratorResult LastResult { get; private set; }

		private readonly WaitForSeconds cachedWaitForSeconds = new WaitForSeconds(CompletionCheckIntervalMilliseconds / 1000f);
		private BaseWorker currentWorker;
		
		private SemaphoreSlim completionSource;

		/// <summary>
		/// Call to make sure current platform is compatible before calling Generate().
		/// </summary>
		/// <returns>True if current platform is supported by the CodeHashGenerator, otherwise returns false.
		/// Can return true in Editor but Hash Generation in Editor is not possible (nothing to hash).</returns>
		public static bool IsTargetPlatformCompatible()
		{
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
			return true;
#else
			return false;
#endif
		}

		/// <summary>
		/// Creates new instance of the CodeHashGenerator at scene if it doesn't exists. Make sure to call NOT from Awake phase.
		/// </summary>
		/// <returns>New or existing instance of the detector.</returns>
		public static ICodeHashGenerator AddToSceneOrGetExisting()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Call to start current runtime code hash generation. Automatically adds instance to the scene if necessary.
		/// </summary>
		/// <param name="maxThreads">Threads to use while hashing the files.</param>
		public static ICodeHashGenerator Generate(int maxThreads = 1)
		{
			return GetOrCreateInstance.GenerateInternal(maxThreads);
		}

		/// <summary>
		/// Awaitable version of Generate(). Allows awaiting for the generation result.
		/// </summary>
		/// <param name="maxThreads">Threads to use while hashing the files.</param>
		public static Task<HashGeneratorResult> GenerateAsync(int maxThreads = 1)
		{
			return GetOrCreateInstance.GenerateInternalAsync(maxThreads);
		}
		
#if UNITY_EDITOR
		// making sure it will reset statics even if domain reload is disabled
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SubsystemRegistration()
		{
			HashGenerated = null;
			Instance = null;
		}
#endif

		/// <summary>
		/// Indicates if hash generation is currently in process.
		/// </summary>
		public bool IsBusy => currentWorker != null && currentWorker.IsBusy;

		ICodeHashGenerator ICodeHashGenerator.Generate(int maxThreads)
		{
			return Generate(maxThreads);
		}
		
		Task<HashGeneratorResult> ICodeHashGenerator.GenerateAsync(int maxThreads)
		{
			return GenerateAsync(maxThreads);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			HashGenerated = null;
		}
		
		private async Task<HashGeneratorResult> GenerateInternalAsync(int maxThreads)
		{
			completionSource = new SemaphoreSlim(0,1);
			GenerateInternal(maxThreads);
			await completionSource.WaitAsync(TimeSpan.FromHours(10));
			completionSource.Dispose();
			completionSource = null;
			
			return LastResult;
		}

		private ICodeHashGenerator GenerateInternal(int maxThreads)
		{
			if (LastResult != null)
			{
				HashGenerated?.Invoke(LastResult);
				completionSource?.Release();
				return this;
			}

			if (IsBusy)
			{
				Debug.LogWarning($"{nameof(CodeHashGenerator)} generation was started while it's already busy.");
				LastResult = HashGeneratorResult.FromError("Already running.");
				completionSource?.Release();
				return this;
			}

			currentWorker = null;
			
#if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_STANDALONE_WIN)
			if (Application.isEditor)
				Debug.LogError(ACTk.LogPrefix +
						   "CodeHashGenerator does not work in Editor. Please use it in Unity Player only.\n" +
						   "This message is harmless.");
			else
				Debug.LogError(ACTk.LogPrefix + "CodeHashGenerator works only in Android and Windows Standalone runtimes (both Mono and IL2CPP).");
			LastResult = HashGeneratorResult.FromError("Incorrect platform.");
			completionSource?.Release();
			return this;
#else
			maxThreads = 1 > maxThreads ? 1 : maxThreads;
	#if UNITY_ANDROID
			currentWorker = new AndroidWorker(maxThreads);
	#elif UNITY_STANDALONE_WIN
			currentWorker = new StandaloneWindowsWorker(maxThreads);
	#endif
			currentWorker.Execute();
			StartCoroutine(CalculationAwaiter());

			return this;
#endif
		}

		private IEnumerator CalculationAwaiter()
		{
			while (currentWorker.IsBusy)
				yield return cachedWaitForSeconds;

			LastResult = currentWorker.Result;
			HashGenerated?.Invoke(LastResult);
			completionSource?.Release();
			currentWorker = null;
		}
	}
}