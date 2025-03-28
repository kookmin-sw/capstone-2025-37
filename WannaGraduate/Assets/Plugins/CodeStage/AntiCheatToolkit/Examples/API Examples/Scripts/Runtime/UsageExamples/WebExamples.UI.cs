#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal partial class WebExamples
	{
		public void DrawUI()
		{
			DrawDomainLockUI();
		}

		private void DrawDomainLockUI()
		{
			GUILayout.Label("<b>Web domain lock</b>");
			GUILayout.Space(10);
			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				GUILayout.Label($"You can detect your app is running from unknown domains.");
				
				GUILayout.Space(5);

				if (Application.platform != RuntimePlatform.WebGLPlayer)
				{
					GUILayout.Label(ExamplesGUI.Colorize("Works only in WebGL builds.", 
							ExamplesGUI.YellowColor));
				}
				else
				{
					
				}
			}
		}
	}
}