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
	internal class SerializedObscuredVector2Int : MigratableSerializedObscuredType<Vector2Int>
	{
		public ObscuredVector2Int.RawEncryptedVector2Int Hidden
		{
			get => new ObscuredVector2Int.RawEncryptedVector2Int 
			{
				x = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2Int.RawEncryptedVector2Int.x)).intValue,
				y = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2Int.RawEncryptedVector2Int.y)).intValue
			};

			set
			{
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2Int.RawEncryptedVector2Int.x)).intValue = value.x;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector2Int.RawEncryptedVector2Int.y)).intValue = value.y;
			}
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override Vector2Int Plain => ObscuredVector2Int.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredVector2Int.Version;
		
		protected override bool PerformMigrate()
		{
			if (Version == 0 || TypeVersion == 1)
			{
				MigrateFromV0();
				Version = TypeVersion;
				return true;
			}
			
			return false;

			void MigrateFromV0()
			{
				var decrypted = ObscuredVector2Int.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredVector2Int.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}
		
		public override string GetMigrationResultString()
		{
			return ObscuredVector2Int.DecryptFromV0(Hidden, Key).ToString();
		}
	}
}

#endif