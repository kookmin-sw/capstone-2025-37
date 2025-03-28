#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System.Globalization;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredDecimal))]
	internal class ObscuredDecimalDrawer : ObscuredTypeDrawer<SerializedObscuredDecimal, decimal>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
#if UNITY_2022_1_OR_NEWER
			var input = EditorGUI.DelayedTextField(position, label, plain.ToString(CultureInfo.InvariantCulture));
#else
			var input = EditorGUI.TextField(position, label, plain.ToString(CultureInfo.InvariantCulture));
#endif
			decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out plain);

		}
		
		protected override void ApplyChanges()
		{
			serialized.Hidden = DecimalLongBytesUnion.FromDecimal(ObscuredDecimal.Encrypt(plain, serialized.Key));
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}