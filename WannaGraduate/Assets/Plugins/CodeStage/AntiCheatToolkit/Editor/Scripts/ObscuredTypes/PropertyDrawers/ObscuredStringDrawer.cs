#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredString))]
	internal class ObscuredStringDrawer : ObscuredTypeDrawer<SerializedObscuredString, string>
	{
		protected override void BeforeOnGUI(ref Rect position, ref SerializedProperty sp, ref GUIContent label)
		{
			base.BeforeOnGUI(ref position, ref sp, ref label);
			
			var size = serialized.HiddenProperty.FindPropertyRelative("Array.size");
			var showMixed = size.hasMultipleDifferentValues;
			
			if (!showMixed)
			{
				for (var i = 0; i < serialized.HiddenProperty.arraySize; i++)
				{
					showMixed |= serialized.HiddenProperty.GetArrayElementAtIndex(i).hasMultipleDifferentValues;
					if (showMixed) break;
				}
			}
			
			if (showMixed)
				EditorGUI.showMixedValue = true;
			
			if (label.text.IndexOf('[') != -1)
			{
				var dataIndex = sp.propertyPath.IndexOf("Array.data[", StringComparison.Ordinal);

				if (dataIndex >= 0)
				{
					dataIndex += 11;
					var index = "Element " + sp.propertyPath.Substring(dataIndex, sp.propertyPath.IndexOf("]", dataIndex, StringComparison.Ordinal) - dataIndex);
					label.text = index;
				}
			}
		}
		
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
#if UNITY_2022_1_OR_NEWER
			plain = EditorGUI.DelayedTextField(position, label, plain);
#else
			plain = EditorGUI.TextField(position, label, plain);
#endif
		}
		
		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredString.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain.ToCharArray());
			
			EditorGUI.showMixedValue = false;
		}
	}
}