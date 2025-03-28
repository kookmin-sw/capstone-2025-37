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
	internal class SerializedObscuredVector2 : SerializedObscuredType<Vector2>
	{
		public ObscuredVector2.RawEncryptedVector2 Hidden
		{
			get => new ObscuredVector2.RawEncryptedVector2 
			{
				x = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2.RawEncryptedVector2.x)).intValue,
				y = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2.RawEncryptedVector2.y)).intValue
			};

			set
			{
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2.RawEncryptedVector2.x)).intValue = value.x;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2.RawEncryptedVector2.y)).intValue = value.y;
			}
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override Vector2 Plain => ObscuredVector2.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredVector2.Version;
	}
}

#endif