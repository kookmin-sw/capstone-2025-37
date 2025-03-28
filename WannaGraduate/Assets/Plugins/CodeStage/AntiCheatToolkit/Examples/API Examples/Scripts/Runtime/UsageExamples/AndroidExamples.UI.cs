#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.Genuine.Android;

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal partial class AndroidExamples
	{
		public void DrawUI()
		{
			DrawAppInstallSourceUI();
			GUILayout.Space(10);
			DrawScreenRecordingUI();
		}

		private void DrawAppInstallSourceUI()
		{
			GUILayout.Label("<b>Android Installation Source Validation</b>");
			GUILayout.Space(10);
			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				GUILayout.Label(
					$"You can figure out where app was installed from using {nameof(AppInstallationSourceValidator)}\n" +
					"This can be used to detect unauthorized redistribution of your app.");
				
				GUILayout.Space(5);

				if (Application.platform != RuntimePlatform.Android)
				{
					GUILayout.Label(ExamplesGUI.Colorize("Works only in Android builds.", 
							ExamplesGUI.YellowColor));
				}
				else
				{
					if (AppInstallationResult != null)
					{
						GUILayout.Label($"Installation source: {AppInstallationResult.PackageName}");
						GUILayout.Label($"Detected store: {AppInstallationResult.DetectedSource}");
					}
					else if (GUILayout.Button("Get Android Installation Source"))
					{
						AppInstallationResult = GetAndroidInstallationSource();
					}
				}
			}
		}
		
		private void DrawScreenRecordingUI()
		{
			GUILayout.Label("<b>Screen Recording prevention</b>");
			GUILayout.Space(10);
			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				GUILayout.Label("This feature asks Android to prevent screenshots and screen recording of your app and hides preview in Task Manager.");
				GUILayout.Label(ExamplesGUI.Colorize("<b>Android makes its best to prevent screenshots and video recording, but it's not guaranteed it will work with some custom ROMs built-in software.</b>", ExamplesGUI.YellowColor));
				GUILayout.Space(5);
				
				if (Application.isEditor || Application.platform != RuntimePlatform.Android)
				{
					GUILayout.Label(ExamplesGUI.Colorize("Works only in Android builds.",
						ExamplesGUI.YellowColor));
					return;
				}

				GUILayout.Label($"Screen recording allowed: <b>{IsScreenRecordingAllowed}</b>");
				if (IsScreenRecordingAllowed)
				{
					if (GUILayout.Button("Prevent and save"))
						SwitchScreenRecording();
				}
				else
				{
					if (GUILayout.Button("Allow and save"))
						SwitchScreenRecording();
				}
			}
		}
	}
}