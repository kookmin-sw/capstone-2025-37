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
	internal class SerializedObscuredInt : MigratableSerializedObscuredType<int>
	{
		public int Hidden
		{
			get => HiddenProperty.intValue;
			set => HiddenProperty.intValue = value;
		}

		public int Key
		{
			get => KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override int Plain => ObscuredInt.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredInt.Version;

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
				var decrypted = ObscuredInt.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredInt.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}

		public override string GetMigrationResultString()
		{
			return ObscuredInt.DecryptFromV0(Hidden, Key).ToString(CultureInfo.InvariantCulture);
		}
	}
}

#endif