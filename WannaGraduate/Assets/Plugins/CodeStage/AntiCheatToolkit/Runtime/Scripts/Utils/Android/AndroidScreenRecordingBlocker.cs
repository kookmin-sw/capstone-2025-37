#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Utils
{
	/// <summary>
	/// Allows preventing screenshots or screen recording of your app using Android's builtin security feature. Can be helpful against some bots on non-rooted devices.
	/// </summary>
	/// While Android makes its best to prevent screenshots and video recording, it's not guaranteed it will work with some custom ROMs built-in software.
	/// Please keep in mind anyone still can use another camera to shoot your app footage from current device screen.
	public static class AndroidScreenRecordingBlocker
	{
		public static void PreventScreenRecording()
		{
#if UNITY_ANDROID
			AndroidRoutines.SetSecureFlag();
#elif DEBUG
			UnityEngine.Debug.LogWarning($"{nameof(AndroidScreenRecordingBlocker)} does work on Android platform only.");
#endif
		}
		
		public static void AllowScreenRecording()
		{
#if UNITY_ANDROID
			AndroidRoutines.RemoveSecureFlag();
#elif DEBUG
			UnityEngine.Debug.LogWarning($"{nameof(AndroidScreenRecordingBlocker)} does work on Android platform only.");
#endif
		}
	}
}