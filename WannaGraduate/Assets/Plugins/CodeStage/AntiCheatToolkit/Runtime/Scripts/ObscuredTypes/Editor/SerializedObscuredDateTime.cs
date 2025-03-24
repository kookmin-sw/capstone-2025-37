#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using System;
using System.Globalization;
using CodeStage.AntiCheat.Utils;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredDateTime : MigratableSerializedObscuredType<DateTime>
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
		public override DateTime Plain => ObscuredDateTime.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredDateTime.Version;

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
				var decrypted = ObscuredDateTime.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredDateTime.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}
		
		public override string GetMigrationResultString()
		{
			return ObscuredDateTime.DecryptFromV0(Hidden, Key).ToString("o", DateTimeFormatInfo.InvariantInfo);
		}
	}
}

#endif