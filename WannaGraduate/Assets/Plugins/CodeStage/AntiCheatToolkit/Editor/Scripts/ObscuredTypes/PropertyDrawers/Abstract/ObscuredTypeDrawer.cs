#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	public abstract class ObscuredTypeDrawer<TSerializedObscuredType, TPlainType> : PropertyDrawer
		where TSerializedObscuredType : SerializedObscuredType<TPlainType>, new()
	{
		protected TSerializedObscuredType serialized;
		protected TPlainType plain;

		public override void OnGUI(Rect position, SerializedProperty sp, GUIContent label)
		{
			serialized = new TSerializedObscuredType();
			serialized.Init(sp);
			plain = serialized.Plain;
			
			BeforeOnGUI(ref position, ref sp, ref label);
			
			var labelWidth = EditorGUIUtility.labelWidth;
			label = EditorGUI.BeginProperty(position, label, sp);

			if (!serialized.IsDataValid)
				DrawFixButton(ref position);

			EditorGUI.BeginChangeCheck();

			DrawProperty(position, sp, label);

			if (EditorGUI.EndChangeCheck())
				ApplyChanges();

			EditorGUI.EndProperty();
			EditorGUIUtility.labelWidth = labelWidth;
		}

		private void DrawFixButton(ref Rect position)
		{
			var fixPosition = EditorGUI.IndentedRect(position);
			var fixButtonRect = new Rect(fixPosition) { width = 40 };
			fixPosition.x += 45;
			position.x += 45;
			position.width -= 45;
			EditorGUIUtility.labelWidth -= 45;
			DrawFixBackground(fixPosition);

			if (GUI.Button(fixButtonRect, new GUIContent("Fix",
						"Fix invalid state with either migrating from older version or overriding hash.")))
			{
				if (serialized.IsCanMigrate)
				{
					var migrated = serialized.GetMigrationResultString();

					EditorApplication.delayCall += () =>
					{
						var option = EditorUtility.DisplayDialogComplex(
							"Anti-Cheat Toolkit",
							$"This variable can be migrated to:\n'{migrated}'.\nDo you wish to migrate?",
							"Migrate",
							"Only fix",
							"Cancel"
						);

						switch (option)
						{
							case 0:
								if (!serialized.Migrate())
									ACTk.PrintExceptionForSupport("Couldn't migrate obscured type instance!");
								break;
							case 1:
								if (!serialized.Fix())
									ACTk.PrintExceptionForSupport("Couldn't fix obscured type instance!");
								break;
							case 2:
								break;
						}
					};
				}
				else
				{
					if (!serialized.Fix()) 
						ACTk.PrintExceptionForSupport("Couldn't fix obscured type instance!");
				}
			}
		}

		protected virtual void DrawFixBackground(Rect position)
		{
			var bgRect = new Rect(position) { width = EditorGUIUtility.labelWidth };
			EditorGUI.DrawRect(bgRect, new Color32(255, 0, 0, 25));
		}

		protected abstract void DrawProperty(Rect position, SerializedProperty sp, GUIContent label);
		protected abstract void ApplyChanges();
		protected virtual void BeforeOnGUI(ref Rect position, ref SerializedProperty sp, ref GUIContent label) { }
	}
}