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
	[CustomPropertyDrawer(typeof(ObscuredQuaternion))]
	internal class ObscuredQuaternionDrawer : WideObscuredTypeDrawer<SerializedObscuredQuaternion, Quaternion>
	{
		protected override void DrawProperty(Rect position, SerializedProperty sp, GUIContent label)
		{
			plain = Vector4ToQuaternion(EditorGUI.Vector4Field(position, label, QuaternionToVector4(plain)));
		}

		protected override void ApplyChanges()
		{
			serialized.Hidden = ObscuredQuaternion.Encrypt(plain, serialized.Key);
			serialized.Hash = HashUtils.CalculateHash(plain);
		}

		private static Vector4 QuaternionToVector4(Quaternion value)
		{
			return new Vector4(value.x, value.y, value.z, value.w);
		}
		
		private static Quaternion Vector4ToQuaternion(Vector4 value)
		{
			return new Quaternion(value.x, value.y, value.z, value.w);
		}
	}
}