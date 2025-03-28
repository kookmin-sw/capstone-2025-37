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
	internal class SerializedObscuredBool : MigratableSerializedObscuredType<bool>
	{
		public int Hidden
		{
			get => HiddenProperty.intValue;
			set => HiddenProperty.intValue = value;
		}

		public byte Key
		{
			get => (byte)KeyProperty.intValue;
			set => KeyProperty.intValue = value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override bool Plain => ObscuredBool.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredBool.Version;

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
				var decrypted = ObscuredBool.DecryptFromV0(Hidden, Key);
				var validHash = HashUtils.CalculateHash(decrypted);
				Hidden = ObscuredBool.Encrypt(decrypted, Key);
				Hash = validHash;
			}
		}

		public override string GetMigrationResultString()
		{
			return ObscuredBool.DecryptFromV0(Hidden, Key).ToString(CultureInfo.InvariantCulture);
		}
	}
}

#endif