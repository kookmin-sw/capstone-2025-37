#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_WEBGL && !UNITY_EDITOR
#define ACTK_WEBGL_BUILD
#endif

namespace CodeStage.AntiCheat.Utils
{
	using System;
	using UnityEngine;

	internal static class TimeUtils
	{
		public const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;

		/// <summary>
		/// Gets speed hacks unbiased current time ticks.
		/// </summary>
		/// <returns>Reliable current time in ticks.</returns>
		public static long GetReliableTicks()
		{
			long ticks = 0;
			
#if !UNITY_EDITOR
#if UNITY_ANDROID
			ticks = TryReadTicksFromAndroidRoutine();
#elif ACTK_WEBGL_BUILD
			ticks = TryReadTicksFromWebGLRoutine();
#endif
#endif
			if (ticks == 0)
				ticks = DateTime.UtcNow.Ticks;

			return ticks;
		}

		public static long GetEnvironmentTicks()
		{
			return Environment.TickCount * TimeSpan.TicksPerMillisecond;
		}

		public static long GetRealtimeTicks()
		{
			return (long)(Time.realtimeSinceStartup * TicksPerSecond);
		}
		
		public static long GetDspTicks()
		{
#if UNITY_AUDIO_MODULE
			return (long)(AudioSettings.dspTime * TicksPerSecond);
#else
			return 0;
#endif
		}

#if UNITY_ANDROID
		private static long TryReadTicksFromAndroidRoutine()
		{
			long result = 0;

			// getting time in nanoseconds from the native Android timer
			// since some random fixed and JVM initialization point
			// (it even may be a future so value could be negative)
			result = AndroidRoutines.GetSystemNanoTime();
			result /= 100;

			return result;
		}
		
#elif ACTK_WEBGL_BUILD
		[System.Runtime.InteropServices.DllImport("__Internal")]
		private static extern double GetUTCTicks();

		private static long TryReadTicksFromWebGLRoutine()
		{
			var ticks = (long)GetUTCTicks();
			return ticks < 0 ? 0 : ticks;
		}
#endif
	}
}