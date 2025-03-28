#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using System.Globalization;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredFloat : MigratableSerializedObscuredType<float>
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
		public override float Plain => ObscuredFloat.Decrypt(Hidden, Key);
		public override bool IsCanMigrate => TryMigrateObsolete(Target, false, out _);
		protected override byte TypeVersion => ObscuredBool.Version;

		protected override bool PerformMigrate()
		{
			return TryMigrateObsolete(Target, true, out _);
		}
		
		private bool TryMigrateObsolete(SerializedProperty sp, bool apply, out float value)
		{
			value = default;
			
			var hiddenValue = sp.FindPropertyRelative(nameof(ObscuredFloat.hiddenValue));
			if (hiddenValue == null)
				return false;

			var migratedVersion = sp.FindPropertyRelative(nameof(ObscuredFloat.migratedVersion));
			if (migratedVersion != null)
			{
				if (migratedVersion.stringValue == ObsoleteMigrationVersion)
					return false;

				migratedVersion.stringValue = ObsoleteMigrationVersion;
			}

			var hiddenValueOldProperty = sp.FindPropertyRelative(nameof(ObscuredFloat.hiddenValueOldByte4));
			var hiddenValueOld = default(ACTkByte4);
			var oldValueExists = false;

			if (hiddenValueOldProperty?.FindPropertyRelative(nameof(ACTkByte4.b1)) != null)
			{
				hiddenValueOld.b1 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b1)).intValue;
				hiddenValueOld.b2 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b2)).intValue;
				hiddenValueOld.b3 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b3)).intValue;
				hiddenValueOld.b4 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b4)).intValue;

				if (hiddenValueOld.b1 != 0 ||
					hiddenValueOld.b2 != 0 ||
					hiddenValueOld.b3 != 0 ||
					hiddenValueOld.b4 != 0)
				{
					oldValueExists = true;
				}
			}

			if (!oldValueExists)
				return false;

			var union = new ObscuredFloat.FloatIntBytesUnion {b4 = hiddenValueOld};
			union.b4.Shuffle();
			
			value = ObscuredFloat.Decrypt(union.i, Key);

			if (apply)
			{
				hiddenValue.intValue = union.i;

				hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b1)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b2)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b3)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ACTkByte4.b4)).intValue = 0;
			}

			return true;
		}
		
		public override string GetMigrationResultString()
		{
			return TryMigrateObsolete(Target, false, out var value) ? value.ToString(CultureInfo.InvariantCulture) : null;
		}
	}
}

#endif