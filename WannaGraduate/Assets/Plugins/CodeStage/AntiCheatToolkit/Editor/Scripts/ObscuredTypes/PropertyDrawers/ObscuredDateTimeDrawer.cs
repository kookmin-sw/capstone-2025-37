#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Globalization;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredDateTime))]
	internal class ObscuredDateTimeDrawer : ObscuredTypeDrawer<SerializedObscuredDateTime, DateTime>
	{
		private string input;

		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			var posW = position.width;
			position.width *= 0.75f;
			
			var kindRect = position;
			kindRect.x = position.xMax + 5;
			kindRect.width = posW - position.width - 5;
			
			var dateString = plain.ToString("o", DateTimeFormatInfo.InvariantInfo);
			input = EditorGUI.DelayedTextField(position, label, dateString);
			
			label = EditorGUI.BeginProperty(kindRect, GUIContent.none, sp);
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				var kind = plain.Kind;
				var kindInput = (DateTimeKind)EditorGUI.EnumPopup(kindRect, label, kind);
				if (scope.changed)
				{
					plain = DateTime.SpecifyKind(plain, kindInput);
					input = plain.ToString("o", DateTimeFormatInfo.InvariantInfo);
					ApplyChanges();
				}
			}
			EditorGUI.EndProperty();
		}

		protected override void ApplyChanges()
		{
			DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out plain);

			serialized.Hidden = ObscuredDateTime.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}
	}
}