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
	internal class SerializedObscuredLong : MigratableSerializedObscuredType<long>
	{
		public long Hidden
		{
			get => HiddenProperty.longValue;
			set => HiddenProperty.longValue = value;
		}

		public long Key
		{
			get => KeyProperty.longValue;
			set => KeyProperty.longValue = value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override long Plain => ObscuredLong.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredLong.Version;

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
				var decrypted = ObscuredLong.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredLong.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}
		
		public override string GetMigrationResultString()
		{
			return ObscuredLong.DecryptFromV0(Hidden, Key).ToString(CultureInfo.InvariantCulture);
		}
	}
}

#endif