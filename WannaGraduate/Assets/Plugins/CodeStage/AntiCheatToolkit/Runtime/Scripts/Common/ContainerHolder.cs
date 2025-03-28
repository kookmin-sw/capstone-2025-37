#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using UnityEngine;

namespace CodeStage.AntiCheat.Common
{
	internal static class ContainerHolder
	{
		public const string ContainerName = "Anti-Cheat Toolkit";
		private static GameObject container;
		private static bool containerSet;
		
#if UNITY_EDITOR
		// making sure it will reset the container even if domain reload is disabled
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SubsystemRegistration()
		{
			containerSet = false;
			container = null;
		}
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void AfterAssembliesLoaded() => AssertNoContainerExists(nameof(RuntimeInitializeLoadType.AfterAssembliesLoaded));

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void BeforeSplashScreen() => AssertNoContainerExists(nameof(RuntimeInitializeLoadType.BeforeSplashScreen));
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeSceneLoad() => AssertNoContainerExists(nameof(RuntimeInitializeLoadType.BeforeSceneLoad));

		private static void AssertNoContainerExists(string phaseName)
		{
			if (!containerSet)
				return;

			Debug.LogError($"[ACTk] {nameof(ContainerHolder)}: container created too early ({phaseName})!\n" +
						   $"It should be created only after {nameof(RuntimeInitializeLoadType)}.{nameof(RuntimeInitializeLoadType.AfterSceneLoad)} / Awake phase to avoid state corruption!\n" +
						   $"Make sure to avoid accessing ACTk APIs before {nameof(RuntimeInitializeLoadType)}.{nameof(RuntimeInitializeLoadType.AfterSceneLoad)} / Awake");
		}

		public static T AddContainerComponent<T>() where T : KeepAliveBehaviour<T>
		{
			if (container == null)
				container = new GameObject(ContainerName);

			containerSet = true;
					
			return container.AddComponent<T>();
		}

		public static void TrySetContainer(GameObject gameObject)
		{
			if (container == null && gameObject.name == ContainerName)
			{
				container = gameObject;
				containerSet = true;
			}
		}
	}
}