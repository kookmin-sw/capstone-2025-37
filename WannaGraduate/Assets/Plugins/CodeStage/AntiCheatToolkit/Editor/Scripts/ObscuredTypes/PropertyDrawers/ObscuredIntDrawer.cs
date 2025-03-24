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
	[CustomPropertyDrawer(typeof(ObscuredInt))]
	internal class ObscuredIntDrawer : ObscuredTypeDrawer<SerializedObscuredInt, int>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
#if UNITY_2022_1_OR_NEWER
			plain = EditorGUI.DelayedIntField(position, label, plain);
#else
			plain = EditorGUI.IntField(position, label, plain);
#endif
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredInt.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}