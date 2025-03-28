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
	[CustomPropertyDrawer(typeof(ObscuredULong))]
	internal class ObscuredULongDrawer : ObscuredTypeDrawer<SerializedObscuredULong, ulong>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			plain = (ulong)EditorGUI.LongField(position, label, (long)plain);
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredULong.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}