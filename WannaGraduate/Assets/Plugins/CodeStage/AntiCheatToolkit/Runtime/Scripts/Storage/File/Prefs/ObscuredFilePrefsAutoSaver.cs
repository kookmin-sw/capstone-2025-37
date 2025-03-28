#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if !UNITY_WEBGL
#define ACTK_ASYNC
#endif

namespace CodeStage.AntiCheat.Storage
{
	using System;
	using Common;
	using UnityEngine;
	using Utils;

	internal static class ObscuredFilePrefsAutoSaver
	{
		private static bool isInited;
		
#if UNITY_EDITOR
		// making sure it will reset statics even if domain reload is disabled
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			isInited = false;
		}
#endif
		
		internal static void Init()
		{
			if (isInited)
				return;

			try
			{
				if (UnityApiResultsHolder.IsMobilePlatform())
				{
					AppEventsDispatcher.Instance.ApplicationFocused += OnApplicationFocused;
					AppEventsDispatcher.Instance.ApplicationPaused += OnApplicationPaused;
				}
				else
				{
					Application.wantsToQuit += OnApplicationWantsToQuit;
				}

#if UNITY_EDITOR
				UnityEditor.EditorApplication.wantsToQuit += OnEditorApplicationWantsToQuit;
				UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
#endif
				isInited = true;
			}
			catch (UnityException e)
			{
				Debug.LogError($"{ACTk.LogPrefix} Couldn't initialize {nameof(ObscuredFilePrefsAutoSaver)}. " +
							   $"Are you using {nameof(ObscuredFilePrefs)} from background thread but didn't " +
							   $"call the {nameof(UnityApiResultsHolder)}.{nameof(UnityApiResultsHolder.InitForAsyncUsage)}()?\n" +
							   $"Auto Save will not be operational!\nException:{e}");
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Something went wrong while creating {nameof(ObscuredFilePrefsAutoSaver)}", e);
			}
		}

#if UNITY_EDITOR
		private static bool OnEditorApplicationWantsToQuit()
		{
			TrySave(false);
			return true;
		}
		
		private static void OnEditorPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
		{
			if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
				TrySave(false);
		}
#endif

		private static bool OnApplicationWantsToQuit()
		{
			TrySave(false);
			return true;
		}

		private static void OnApplicationFocused(bool hasFocus)
		{
			if (!hasFocus)
				TrySave();
		}

		private static void OnApplicationPaused(bool pauseStatus)
		{
			if (pauseStatus)
				TrySave();
		}
		
		private static void TrySave(bool async = true)
		{
			try
			{
#if ACTK_ASYNC
				if (async)
					System.Threading.Tasks.Task.Run(Save);
				else
					Save();
#else
				Save();
#endif
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"{nameof(ObscuredFilePrefsAutoSaver)} couldn't save data.", e);
			}

			void Save()
			{
				if (ObscuredFilePrefs.IsInited &&
					!ObscuredFilePrefs.IsSaved && 
					!ObscuredFilePrefs.IsBusy)
					ObscuredFilePrefs.Save();
			}
		}
	}
}