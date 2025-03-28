#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.Editors
{
	using Detectors;

	using UnityEditor;
	using UnityEditor.EditorTools;
	using UnityEngine;
	using EditorTools = EditorCode.EditorTools;

	[CustomEditor(typeof (SpeedHackDetector))]
	internal class SpeedHackDetectorEditor : KeepAliveBehaviourEditor<SpeedHackDetector>
	{
		private SerializedProperty interval;
		private SerializedProperty threshold;
		private SerializedProperty maxFalsePositives;
		private SerializedProperty coolDown;
		private SerializedProperty useDsp;

		protected override void FindUniqueDetectorProperties()
		{
			interval = serializedObject.FindProperty("interval");
			threshold = serializedObject.FindProperty("threshold");
			maxFalsePositives = serializedObject.FindProperty("maxFalsePositives");
			coolDown = serializedObject.FindProperty("coolDown");
			
			useDsp = serializedObject.GetProperty(nameof(SpeedHackDetector.UseDsp));
		}

		protected override bool DrawUniqueDetectorProperties()
		{
			DrawHeader("Specific settings");

			EditorGUILayout.PropertyField(interval);
			EditorGUILayout.PropertyField(threshold);
			EditorGUILayout.PropertyField(maxFalsePositives);
			EditorGUILayout.PropertyField(coolDown);

#if UNITY_2020_1_OR_NEWER
			EditorGUILayout.PropertyField(useDsp);
#else
			EditorGUILayout.PropertyField(useDsp, new GUIContent( ObjectNames.NicifyVariableName(nameof(SpeedHackDetector.UseDsp)), useDsp.tooltip));
#endif
			if (useDsp.boolValue) 
				EditorGUILayout.HelpBox("Dsp timers may cause false positives on some hardware.\nMake sure to test on target devices before using this in production.", MessageType.Warning);
			
#if UNITY_AUDIO_MODULE
			if (!EditorTools.IsAudioManagerEnabled())
			{
				EditorGUILayout.HelpBox("Dsp option is not available since Disable Unity Audio option is enabled.", MessageType.Error);
				if (GUILayout.Button("Open Audio Settings"))
				{
					SettingsService.OpenProjectSettings("Project/Audio");
#if UNITY_2021_3_OR_NEWER
					Highlighter.Highlight("Project Settings", EditorTools.GetAudioManagerEnabledPropertyPath(), HighlightSearchMode.Identifier);
#endif
				}
			}
#else
			EditorGUILayout.HelpBox("Dsp option is not available since built-in Audio module is disabled.", MessageType.Error);
#endif
			return true;
		}
	}
}