#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	public abstract class WideObscuredTypeDrawer<TSerializedObscuredType, TPlainType> : ObscuredTypeDrawer<TSerializedObscuredType, TPlainType>
		where TSerializedObscuredType : SerializedObscuredType<TPlainType>, new()
	{
		protected override void DrawFixBackground(Rect position)
		{
			if (EditorGUIUtility.wideMode)
				base.DrawFixBackground(position);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2f;
		}
	}
}