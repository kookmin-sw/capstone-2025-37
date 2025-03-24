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
	internal class SerializedObscuredVector3Int : MigratableSerializedObscuredType<Vector3Int>
	{
		public ObscuredVector3Int.RawEncryptedVector3Int Hidden
		{
			get => new ObscuredVector3Int.RawEncryptedVector3Int 
			{
				x = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.x)).intValue,
				y = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.y)).intValue,
				z = HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.z)).intValue
			};

			set
			{
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.x)).intValue = value.x;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.y)).intValue = value.y;
				HiddenProperty.FindPropertyRelative(nameof(ObscuredVector3Int.RawEncryptedVector3Int.z)).intValue = value.z;
			}
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override Vector3Int Plain => ObscuredVector3Int.Decrypt(Hidden, Key);
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
				var decrypted = ObscuredVector3Int.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredVector3Int.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}
		
		public override string GetMigrationResultString()
		{
			return ObscuredVector3Int.DecryptFromV0(Hidden, Key).ToString();
		}
	}
}

#endif