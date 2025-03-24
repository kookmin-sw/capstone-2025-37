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
	[CustomPropertyDrawer(typeof(ObscuredVector3))]
	internal class ObscuredVector3Drawer : WideObscuredTypeDrawer<SerializedObscuredVector3, Vector3>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			plain = EditorGUI.Vector3Field(position, label, plain);
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredVector3.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}