#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredVector2))]
	internal class ObscuredVector2Drawer : WideObscuredTypeDrawer<SerializedObscuredVector2, Vector2>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			plain = EditorGUI.Vector2Field(position, label, plain);
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredVector2.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}