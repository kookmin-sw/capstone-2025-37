#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using System.Globalization;
using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredUInt : MigratableSerializedObscuredType<uint>
	{
		public uint Hidden
		{
			get => (uint)HiddenProperty.longValue;
			set => HiddenProperty.longValue = value;
		}

		public uint Key
		{
			get => (uint)KeyProperty.longValue;
			set => KeyProperty.longValue = value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override uint Plain => ObscuredUInt.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredUInt.Version;
		
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
				var decrypted = ObscuredUInt.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredUInt.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}
		
		public override string GetMigrationResultString()
		{
			return ObscuredUInt.DecryptFromV0(Hidden, Key).ToString(CultureInfo.InvariantCulture);
		}
	}
}

#endif