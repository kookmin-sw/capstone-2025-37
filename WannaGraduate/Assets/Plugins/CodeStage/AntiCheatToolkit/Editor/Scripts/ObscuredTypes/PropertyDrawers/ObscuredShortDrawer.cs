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
	[CustomPropertyDrawer(typeof(ObscuredShort))]
	internal class ObscuredShortDrawer : ObscuredTypeDrawer<SerializedObscuredShort, short>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
#if UNITY_2022_1_OR_NEWER
			plain = (short)EditorGUI.DelayedIntField(position, label, plain);
#else
			plain = (short)EditorGUI.IntField(position, label, plain);
#endif
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredShort.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}