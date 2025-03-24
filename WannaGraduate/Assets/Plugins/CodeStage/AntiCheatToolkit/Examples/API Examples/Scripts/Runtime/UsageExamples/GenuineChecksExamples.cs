#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using UnityEngine;
using CodeStage.AntiCheat.Genuine.Android;
using CodeStage.AntiCheat.Genuine.CodeHash;

namespace CodeStage.AntiCheat.Examples
{
	internal partial class GenuineChecksExamples : MonoBehaviour
	{
		private HashGeneratorResult BuildHashResult { get; set; }

		private static bool IsHashingBusy => CodeHashGenerator.Instance.IsBusy;

		private static bool IsHashingSupported => CodeHashGenerator.IsTargetPlatformCompatible();
		
		private void Init()
		{
			// Just to make sure it's added to the scene and Instance will be not empty.
			CodeHashGenerator.AddToSceneOrGetExisting();
		}

		// Event-driven hash generation example
		private void StartGeneration()
		{
			// You can check if current platform is supported first.
			if (!CodeHashGenerator.IsTargetPlatformCompatible())
				return;
			
			// This is a good practice to avoid new requests while generator is busy with previous requests.
			if (CodeHashGenerator.Instance.IsBusy)
				return;
			
			// Just subscribe to generation event and start generation.
			// Generation runs in separate thread avoiding cpu spikes in main thread.
			// It generates hash only once and cache it for any new requests since compiled code does not change in runtime.
			CodeHashGenerator.HashGenerated += OnHashGenerated;

			// using all available cores except 1 (we need to keep it free for the Main / UI thread)
			var availableCores = Math.Max(1, Environment.ProcessorCount - 1);
			
			// calling hash generation to get result using HashGenerated event
			CodeHashGenerator.Generate(availableCores);
		}
		
		// Async hash generation example
		private async void StartGenerationAsync()
		{
			// You can check if current platform is supported first.
			if (!CodeHashGenerator.IsTargetPlatformCompatible())
				return;
			
			// This is a good practice to avoid new requests while generator is busy with previous requests.
			if (CodeHashGenerator.Instance.IsBusy)
				return;

			// using all available cores except 1 (we need to keep it free for the Main / UI thread)
			var availableCores = Math.Max(1, Environment.ProcessorCount - 1);
			
			// You can await generation result and process it right away.
			// Generation runs in separate thread avoiding cpu spikes in main thread.
			// It generates hash only once and cache it for any new requests since hashed binaries do not change at runtime.
			var generationResult = await CodeHashGenerator.GenerateAsync(availableCores);
			
			// re-using event listener to avoid code duplication
			OnHashGenerated(generationResult);
		}

		// Generated hash processing example
		private void OnHashGenerated(HashGeneratorResult result)
		{
			BuildHashResult = result;
			CodeHashGenerator.HashGenerated -= OnHashGenerated;

			if (result.Success)
			{
				Debug.Log($"Files hashed: {result.FileHashes.Count} in {result.DurationSeconds:F2} secs");
				foreach (var fileHash in result.FileHashes)
				{
					Debug.Log(fileHash.ToString());
				}
				
				// Here you can upload your hashes to the server to make a validation check on the server side and punish cheater with server logic.
				//
				// This is a preferred use case since cheater will have to figure out proper hash using
				// packets sniffing (https packets harder to sniff) or debugging first to fake it on the client side requiring more
				// skills and motivation from cheater.
				//
				// check SummaryHash first and if it differs (it can if your runtime build has only portion of the initial build you made in Unity)
				// check FileHashes if SummaryHash differs to see if runtime build have any new hashes - it will indicate build is altered
				//
				// UploadHashes(result.SummaryHash, result.FileHashes);

				// Or you may compare it with hardcoded hashes if you did save them somewhere in the build previously.
				//
				// This is less preferred way since cheater can still try to hack your client-side validation check to make it always pass.
				// Anyways, this is better than nothing and will require some additional time from cheater reducing overall motivation to hack your game.
				// In case implementing it fully on the client side, make sure to compile IL2CPP build and use code
				// obfuscation which runs integrated into the Unity build process so hashing will happen AFTER code obfuscation.
				// If obfuscation will happen after hashing it will change code hash and you'll need to re-calculate it
				// using Tools > Code Stage > Anti-Cheat Toolkit > Calculate external build hash feature.
				//
				// if (!CompareHashes(result.SummaryHash, result.FileHashes))
				// {
				//		Debug.Log("You patched my code, cheater!");
				// }
			}
			else
			{
				Debug.LogError("Oh, something went wrong while getting the hash!\n" +
				               "Error message: " + result.ErrorMessage);
			}
		}
	}
}