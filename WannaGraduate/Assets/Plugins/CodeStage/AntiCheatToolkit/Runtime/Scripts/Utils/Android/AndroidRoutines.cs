#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if UNITY_ANDROID

using System;
using CodeStage.AntiCheat.Common;
using UnityEngine;

namespace CodeStage.AntiCheat.Utils
{
	internal static class AndroidRoutines
	{
		private const string RoutinesClassPath = "net.codestage.actk.androidnative.ACTkAndroidRoutines";
		private const string UnityPlayerClassPath = "com.unity3d.player.UnityPlayer";

		private const string GetSystemNanoTimeMethod = "GetSystemNanoTime";
		private const string GetPackageInstallerNameMethod = "GetPackageInstallerName";

		private static readonly Lazy<AndroidJavaClass> RoutinesClass = new Lazy<AndroidJavaClass>(() => InitJavaClass(RoutinesClassPath));

		public static long GetSystemNanoTime()
		{
			try
			{
				return RoutinesClass.Value?.CallStatic<long>(GetSystemNanoTimeMethod) ?? DateTime.UtcNow.Ticks;
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't call static method {GetSystemNanoTimeMethod} from the {RoutinesClassPath}!\n" +
											  "Make sure its name is not obfuscated and was called from Android Player.", e);
				return default;
			}
		}
		
		public static string GetPackageInstallerName()
		{
			try
			{
				return RoutinesClass.Value?.CallStatic<string>(GetPackageInstallerNameMethod);
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't call static method {GetPackageInstallerNameMethod} from the {RoutinesClassPath}!\n" +
											  "Make sure its name is not obfuscated and was called from Android Player.", e);
				return default;
			}
		}
		
		public static void SetSecureFlag()
		{
			try
			{
				using (AndroidJavaClass unityPlayer = InitJavaClass(UnityPlayerClassPath))
				{
					var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					RoutinesClass.Value?.CallStatic("SetFlagSecure", currentActivity);
				}
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't call code at the {RoutinesClassPath}!\n" +
											  "Make sure its name is not obfuscated and was called from Android Player.", e);
			}
		}	
	
		public static void RemoveSecureFlag()
		{
			try
			{
				using (AndroidJavaClass unityPlayer = InitJavaClass(UnityPlayerClassPath))
				{
					var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					RoutinesClass.Value?.CallStatic("RemoveFlagSecure", currentActivity);
				}
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't call code at the {RoutinesClassPath}!\n" +
											  "Make sure its name is not obfuscated and was called from Android Player.", e);
			}
		}
		
		private static AndroidJavaClass InitJavaClass(string classPath)
		{
			try
			{
				return new AndroidJavaClass(classPath);
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't create instance of the {nameof(AndroidJavaClass)}: {classPath}!\n" +
											  "Please make sure you are not obfuscating public ACTk's Java Plugin classes.", e);
			}
			return null;
		}
	}
}

#endif