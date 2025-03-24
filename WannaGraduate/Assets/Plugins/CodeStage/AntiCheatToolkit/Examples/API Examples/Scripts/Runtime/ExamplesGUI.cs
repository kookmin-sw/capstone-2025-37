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

	[AddComponentMenu("")]
	internal class ExamplesGUI : MonoBehaviour
	{
		private enum ExamplePage
		{
			ObscuredTypes,
			SavesProtection,
			Detectors,
			CodeHashing,
			AndroidOnly,
			WebOnly
		}

		[SerializeField] private int fontSize = 20;

		internal const string RedColor = "FF6060";
		internal const string YellowColor = "E9D604";
		internal const string GreenColor = "02C85F";
		internal const string BlueColor = "75C4EB";
		
		internal static GUIStyle centeredStyle;
		internal static GUIStyle rightAlignStyle;

		private ObscuredTypesExamples obscuredTypesExamples;
		private ObscuredPrefsExamples obscuredPrefsExamples;
		private ObscuredFilePrefsExamples obscuredFilePrefsExamples;
		private DetectorsExamples detectorsExamples;
		private GenuineChecksExamples genuineChecksExamples;
		private AndroidExamples androidExamples;
		private WebExamples webExamples;

		private readonly string[] tabs = {"Memory", 
			"Saves", 
			"Detectors", 
			"Genuine",
			"Android"/*,
			"Web"*/
		};
		private readonly string[] savesTabs = {"ObscuredPrefs", "ObscuredFilePrefs"};
		private ExamplePage currentPage;
		private int savesPage;

		public DetectorsExamples DetectorsExamples => detectorsExamples;

		private void Awake()
		{
			obscuredTypesExamples = GetComponent<ObscuredTypesExamples>();
			obscuredPrefsExamples = GetComponent<ObscuredPrefsExamples>();
			obscuredFilePrefsExamples = GetComponent<ObscuredFilePrefsExamples>();
			detectorsExamples = GetComponent<DetectorsExamples>();
			genuineChecksExamples = GetComponent<GenuineChecksExamples>();
			androidExamples = GetComponent<AndroidExamples>();
			webExamples = GetComponent<WebExamples>();
		}

		private void OnGUI()
		{		
			GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = fontSize;
			
			if (centeredStyle == null)
				centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter};
			
			if (rightAlignStyle == null)
				rightAlignStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperRight};

			GUILayout.BeginArea(new Rect(10, 5, Screen.width - 20, Screen.height - 10));

			GUILayout.Label(Colorize("<b>Anti-Cheat Toolkit Sandbox</b>", BlueColor), centeredStyle);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", centeredStyle);
			GUILayout.Space(5);
			currentPage = (ExamplePage)GUILayout.Toolbar((int)currentPage, tabs);
			GUILayout.Space(25);

			switch (currentPage)
			{
				case ExamplePage.ObscuredTypes:
				{
					obscuredTypesExamples.DrawUI(this);
					break;
				}
				case ExamplePage.SavesProtection:
				{
					DrawSavesProtectionPage();
					break;
				}
				case ExamplePage.Detectors:
				{
					detectorsExamples.DrawUI();
					break;
				}
				case ExamplePage.CodeHashing:
				{
					genuineChecksExamples.DrawUI();
					break;
				}
				case ExamplePage.AndroidOnly:
				{
					androidExamples.DrawUI();
					break;
				}
				case ExamplePage.WebOnly:
				{
					webExamples.DrawUI();
					break;
				}
			}
			GUILayout.EndArea();
		}

		private void DrawSavesProtectionPage()
		{
			savesPage = GUILayout.Toolbar(savesPage, savesTabs);
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(GUILayout.MinWidth(130)))
				{
					GUILayout.Label("<b>Supported types:</b>");
					GUILayout.Label(GetAllObscuredPrefsDataTypes(), GUILayout.MaxWidth(100));
				}
				
				GUILayout.Space(10);
				
				using (new GUILayout.VerticalScope())
				{
					switch (savesPage)
					{
						case 0:
						{
							obscuredPrefsExamples.DrawUI();
							break;
						}
						case 1:
						{
							obscuredFilePrefsExamples.DrawUI();
							break;
						}
					}
				}
			}
		}

		internal static string ColorizeGreenOrRed(string stringToWrap, bool green)
		{
			return Colorize(stringToWrap, green ? GreenColor : RedColor);
		}

		internal static string Colorize(string stringToWrap, string color)
		{
			return $"<color=#{color}>{stringToWrap}</color>";
		}
		
		private static string GetAllObscuredPrefsDataTypes()
		{
			var result = string.Empty;
			var values = Enum.GetNames(typeof(StorageDataType));

			for (var i = 0; i < values.Length; i++)
			{
				var value = values[i];
				var lowerCase = value.ToLowerInvariant();

				if (lowerCase.Contains(StorageDataType.Unknown.ToString().ToLowerInvariant()))
					continue;

				result += lowerCase;
				if (i != values.Length - 1)
					result += ", ";
			}

			result = Colorize(result, BlueColor);

			return result;
		}
    }
}