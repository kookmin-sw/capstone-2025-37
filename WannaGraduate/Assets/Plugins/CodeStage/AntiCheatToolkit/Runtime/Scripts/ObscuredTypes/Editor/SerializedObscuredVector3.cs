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
	internal class SerializedObscuredVector3 : SerializedObscuredType<Vector3>
	{
		public ObscuredVector3.RawEncryptedVector3 Hidden
		{
			get => new ObscuredVector3.RawEncryptedVector3 
			{
				x = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.x)).intValue,
				y = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.y)).intValue,
				z = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.z)).intValue
			};

			set
			{
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.x)).intValue = value.x;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.y)).intValue = value.y;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3.RawEncryptedVector3.z)).intValue = value.z;
			}
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override Vector3 Plain => ObscuredVector3.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredVector3.Version;
	}
}

#endif