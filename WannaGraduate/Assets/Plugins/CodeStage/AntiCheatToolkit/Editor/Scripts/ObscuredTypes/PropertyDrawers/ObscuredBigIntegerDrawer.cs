#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System.Numerics;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredBigInteger))]
	internal class ObscuredBigIntegerDrawer : ObscuredTypeDrawer<SerializedObscuredBigInteger, BigInteger>
	{
		private string input;
		
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
#if UNITY_2022_1_OR_NEWER
			input = EditorGUI.DelayedTextField(position, label, plain.ToString());
#else
			input = EditorGUI.TextField(position, label, plain.ToString());
#endif
		}

		protected override void ApplyChanges()
		{
			if (!BigInteger.TryParse(input, out var newValue))
				newValue = 0;

			plain = newValue;
			
			serialized.Hidden = ObscuredBigInteger.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}