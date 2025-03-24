#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.Examples
{
	using System.Linq;
	using ObscuredTypes;
	using UnityEngine;

	internal partial class ObscuredTypesExamples
	{
		private string allSimpleObscuredTypes;
		
		public void DrawUI(ExamplesGUI examplesGUIInstance)
		{
			GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).");
			GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):");
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label("<b>Obscured types:</b>", GUILayout.MinWidth(130));
					GUILayout.Label($"{ExamplesGUI.Colorize($"{Get()}", ExamplesGUI.BlueColor)}", GUILayout.MinWidth(130));
				}
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					GUILayout.Label($"<b>{ExamplesGUI.Colorize("Regular", ExamplesGUI.RedColor)} types</b>", ExamplesGUI.centeredStyle);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>int:</b>\n{regularInt}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							regularInt += Random.Range(1, 100);
						if (GUILayout.Button("Reset"))
							regularInt = 0;
					}
					
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>float:</b>\n{regularFloat}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							regularFloat += Random.Range(1f, 100f);
						if (GUILayout.Button("Reset"))
							regularFloat = 0;
					}
					
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>Vector3:</b>\n{regularVector3}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							regularVector3 += Random.insideUnitSphere;
						if (GUILayout.Button("Reset"))
							regularVector3 = Vector3.zero;
					}

					GUILayout.Space(10);
					GUILayout.Label($"<b>{ExamplesGUI.Colorize("Obscured", ExamplesGUI.GreenColor)} types</b>", ExamplesGUI.centeredStyle);
					using (new GUILayout.HorizontalScope())
					{
						ObscuredCheatingDetector.Instance.honeyPot =
							GUILayout.Toggle(ObscuredCheatingDetector.Instance.honeyPot, "Honeypot", GUILayout.ExpandWidth(false));

						var oldRandomize = randomize;
						randomize = GUILayout.Toggle(randomize, "Randomize crypto keys");
						if (randomize != oldRandomize)
						{
							if (randomize)
								StartRandomization();
							else
								StopRandomization();
						}
						
						var detected = examplesGUIInstance.DetectorsExamples.obscuredTypeCheatDetected;
						GUILayout.Label($"cheating detected: {ExamplesGUI.ColorizeGreenOrRed(detected.ToString(), !detected)}", ExamplesGUI.rightAlignStyle);
					}
					
					GUILayout.Space(5);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredInt:</b>\n{obscuredInt}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							obscuredInt += Random.Range(1, 100);
						if (GUILayout.Button("Reset"))
							obscuredInt = 0;
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredFloat:</b>\n{obscuredFloat}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							obscuredFloat += Random.Range(1f, 100f);
						if (GUILayout.Button("Reset"))
							obscuredFloat = 0;
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label($"<b>ObscuredVector3:</b>\n{obscuredVector3}", GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
							obscuredVector3 += Random.insideUnitSphere;
						if (GUILayout.Button("Reset"))
							obscuredVector3 = Vector3.zero;
					}
				}
			}
		}
		
		private string Get()
		{
			var result = "Can't use reflection here, sorry :(";
			var types = "";

			if (string.IsNullOrEmpty(allSimpleObscuredTypes))
			{
				var csharpAssembly = typeof(ObscuredInt).Assembly;
				var q = from t in csharpAssembly.GetTypes()
					where t.IsPublic && t.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && t.Name != "IObscuredType"
					select t;
				q.ToList().ForEach(t =>
				{
					if (types.Length > 0)
					{
						types += "\n" + t.Name;
					}
					else
					{
						types += t.Name;
					}
				});

				if (!string.IsNullOrEmpty(types))
				{
					result = types;
					allSimpleObscuredTypes = types;
				}
				else
				{
					allSimpleObscuredTypes = result;
				}
			}
			else
			{
				result = allSimpleObscuredTypes;
			}
			return result;
		}
	}
}