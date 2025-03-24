#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using System;
	using Storage;
	using UnityEngine;

	internal partial class ObscuredPrefsExamples
	{
		private DeviceLockLevel savesLock;
		private DeviceLockTamperingSensitivity tamperingSensitivity;
		
		public void DrawUI()
		{
			GUILayout.Label($"ACTk has secure layer for the PlayerPrefs: {ExamplesGUI.Colorize(nameof(ObscuredPrefs), ExamplesGUI.BlueColor)}. " +
			                "It protects data from view, detects any cheating attempts, " +
			                "optionally locks data to the current device and supports additional data types.");
			GUILayout.Space(5);
			GUILayout.Label("Below you can try to cheat both regular PlayerPrefs and secure ObscuredPrefs:");
			GUILayout.Space(5);
			using (new GUILayout.VerticalScope())
			{
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					GUILayout.Label($"{ExamplesGUI.Colorize($"<b>{nameof(PlayerPrefs)}:</b>", ExamplesGUI.RedColor)}\n" +
									"easy to cheat, only 3 supported types");
					GUILayout.Space(5);
					if (string.IsNullOrEmpty(regularPrefs))
					{
						LoadRegularPrefs();
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label(regularPrefs, GUILayout.Width(450));
						using (new GUILayout.VerticalScope())
						{
							using (new GUILayout.HorizontalScope())
							{
								if (GUILayout.Button("Save"))
									SaveRegularPrefs();
								if (GUILayout.Button("Load"))
									LoadRegularPrefs();
							}
							if (GUILayout.Button("Delete"))
								DeleteRegularPrefs();
						}
					}
				}
				GUILayout.Space(5);
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					GUILayout.Label($"{ExamplesGUI.ColorizeGreenOrRed($"<b>{nameof(ObscuredPrefs)}:</b>", true)}\n" +
									"secure, lot of additional types and extra options");
					GUILayout.Space(5);
					if (string.IsNullOrEmpty(obscuredPrefs))
						LoadObscuredPrefs();

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label(obscuredPrefs, GUILayout.Width(450));
						using (new GUILayout.VerticalScope())
						{
							using (new GUILayout.HorizontalScope())
							{
								if (GUILayout.Button("Save"))
									SaveObscuredPrefs();
								if (GUILayout.Button("Load"))
									LoadObscuredPrefs();
							}
							if (GUILayout.Button("Delete"))
								DeleteObscuredPrefs();

							using (new GUILayout.HorizontalScope())
							{
								GUILayout.Label("LockToDevice level");
							}

							savesLock = (DeviceLockLevel)GUILayout.SelectionGrid((int)savesLock, Enum.GetNames(typeof(DeviceLockLevel)), 3);
							ApplyDeviceLockLevel(savesLock);
							
							using (new GUILayout.HorizontalScope())
							{
								GUILayout.Label("TamperingSensitivity level");
							}

							tamperingSensitivity = (DeviceLockTamperingSensitivity)GUILayout.SelectionGrid((int)tamperingSensitivity, Enum.GetNames(typeof(DeviceLockTamperingSensitivity)), 3);
							ApplyTamperingSensitivity(tamperingSensitivity);

							GUILayout.Space(5);
							using (new GUILayout.HorizontalScope())
							{
								PreservePlayerPrefs = GUILayout.Toggle(PreservePlayerPrefs, "preservePlayerPrefs");
							}
							GUILayout.Space(5);
							GUILayout.Label(ExamplesGUI.ColorizeGreenOrRed("Saves modification detected: " + savesAlterationDetected, !savesAlterationDetected));
							GUILayout.Label(ExamplesGUI.ColorizeGreenOrRed("Foreign saves detected: " + foreignSavesDetected, !foreignSavesDetected));
						}
					}
				}
				GUILayout.Space(5);
			}
		}
	}
}