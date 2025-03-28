#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using System.Globalization;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct LongBytesUnion
	{
		[FieldOffset(0)]
		public readonly long l;

		[FieldOffset(0)]
		public ACTkByte8 b8;
	}
	
	internal class SerializedObscuredDouble : MigratableSerializedObscuredType<double>
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
		public override double Plain => ObscuredDouble.Decrypt(Hidden, Key);
		public override bool IsCanMigrate => TryMigrateObsolete(Target, false, out _);
		protected override byte TypeVersion => ObscuredDouble.Version;

		protected override bool PerformMigrate()
		{
			return TryMigrateObsolete(Target, true, out _);
		}
		
		private bool TryMigrateObsolete(SerializedProperty sp, bool apply, out double value)
		{
			value = default;
			
			var hiddenValue = sp.FindPropertyRelative(nameof(ObscuredDouble.hiddenValue));
			if (hiddenValue == null)
				return false;

			var migratedVersion = sp.FindPropertyRelative(nameof(ObscuredDouble.migratedVersion));
			if (migratedVersion != null)
			{
				if (migratedVersion.stringValue == ObsoleteMigrationVersion)
					return false;

				migratedVersion.stringValue = ObsoleteMigrationVersion;
			}
			
			var hiddenValueOldProperty = sp.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8));
			var hiddenValueOld = default(ACTkByte8);
			var oldValueExists = false;

			if (hiddenValueOldProperty?.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b1)) != null)
			{
				hiddenValueOld.b1 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b1)).intValue;
				hiddenValueOld.b2 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b2)).intValue;
				hiddenValueOld.b3 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b3)).intValue;
				hiddenValueOld.b4 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b4)).intValue;
				hiddenValueOld.b5 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b5)).intValue;
				hiddenValueOld.b6 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b6)).intValue;
				hiddenValueOld.b7 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b7)).intValue;
				hiddenValueOld.b8 = (byte)hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b8)).intValue;

				if (hiddenValueOld.b1 != 0 ||
					hiddenValueOld.b2 != 0 ||
					hiddenValueOld.b3 != 0 ||
					hiddenValueOld.b4 != 0 ||
					hiddenValueOld.b5 != 0 ||
					hiddenValueOld.b6 != 0 ||
					hiddenValueOld.b7 != 0 ||
					hiddenValueOld.b8 != 0)
				{
					oldValueExists = true;
				}
			}

			if (!oldValueExists)
				return false;

			var union = new LongBytesUnion {b8 = hiddenValueOld};
			union.b8.Shuffle();

			value = ObscuredDouble.Decrypt(union.l, Key);
			
			if (apply)
			{
				hiddenValue.longValue = union.l;

				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b1)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b2)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b3)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b4)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b5)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b6)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b7)).intValue = 0;
				hiddenValueOldProperty.FindPropertyRelative(nameof(ObscuredDouble.hiddenValueOldByte8.b8)).intValue = 0;
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