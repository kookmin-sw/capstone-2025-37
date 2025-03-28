#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using CodeStage.AntiCheat.Utils;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredQuaternion : SerializedObscuredType<Quaternion>
	{
		public ObscuredQuaternion.RawEncryptedQuaternion Hidden
		{
			get => new ObscuredQuaternion.RawEncryptedQuaternion 
			{
				x = HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.x)).intValue,
				y = HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.y)).intValue,
				z = HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.z)).intValue,
				w = HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.w)).intValue
			};

			set
			{
				HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.x)).intValue = value.x;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.y)).intValue = value.y;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.z)).intValue = value.z;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredQuaternion.RawEncryptedQuaternion.w)).intValue = value.w;
			}
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override Quaternion Plain => ObscuredQuaternion.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredQuaternion.Version;
	}
}

#endif