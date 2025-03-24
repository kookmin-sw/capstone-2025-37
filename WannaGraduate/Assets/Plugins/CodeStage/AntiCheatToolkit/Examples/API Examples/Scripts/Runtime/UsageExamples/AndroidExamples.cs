#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using UnityEngine;
using CodeStage.AntiCheat.Genuine.Android;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.Utils;

namespace CodeStage.AntiCheat.Examples
{
	internal partial class AndroidExamples : MonoBehaviour
	{
		private AppInstallationSource AppInstallationResult { get; set; }
		private bool IsScreenRecordingAllowed { get; set; } = true;

		private void Awake()
		{
			if (!Application.isEditor)
			{
				IsScreenRecordingAllowed = ObscuredPrefs.Get(nameof(IsScreenRecordingAllowed), true);
				ApplyIsScreenRecordingAllowed();
			}
		}

		private void SwitchScreenRecording()
		{
			IsScreenRecordingAllowed = !IsScreenRecordingAllowed;
			ObscuredPrefs.Set(nameof(IsScreenRecordingAllowed), IsScreenRecordingAllowed);
			ApplyIsScreenRecordingAllowed();
		}

		private void ApplyIsScreenRecordingAllowed()
		{
			if (IsScreenRecordingAllowed)
				AndroidScreenRecordingBlocker.AllowScreenRecording();
			else
				AndroidScreenRecordingBlocker.PreventScreenRecording();
		}
		
		private AppInstallationSource GetAndroidInstallationSource()
		{
			var installationSource = AppInstallationSourceValidator.GetAppInstallationSource();
			if (installationSource.DetectedSource == AndroidAppSource.AccessError)
			{
				Debug.LogError("Failed to detect installation source");
			}
			else
			{
				Debug.Log($"Installed from: {installationSource.DetectedSource} (package name: {installationSource.PackageName})");
				
				if (installationSource.DetectedSource != AndroidAppSource.GooglePlayStore)
					Debug.LogWarning("App was installed not from the Google Play Store!");
			}
			
			return installationSource;
		}
	}
}