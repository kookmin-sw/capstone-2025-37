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
	[CustomPropertyDrawer(typeof(ObscuredVector3Int))]
	internal class ObscuredVector3IntDrawer : WideObscuredTypeDrawer<SerializedObscuredVector3Int, Vector3Int>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			plain = EditorGUI.Vector3IntField(position, label, plain);
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredVector3Int.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}