#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredDecimal : SerializedObscuredType<decimal>
	{
		public DecimalLongBytesUnion Hidden
		{
			get => ReadACTkByte16(HiddenProperty);
			set => WriteACTkByte16(HiddenProperty, value);
		}

		public long Key
		{
			get => KeyProperty.longValue;
			set => KeyProperty.longValue = value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override decimal Plain => ObscuredDecimal.Decrypt(Hidden.d, Key);
		protected override byte TypeVersion => ObscuredDecimal.Version;
		
		private static DecimalLongBytesUnion ReadACTkByte16(SerializedProperty sp)
		{
			var value1 = sp.FindPropertyRelative(nameof(ACTkByte16.b1));
			var value2 = sp.FindPropertyRelative(nameof(ACTkByte16.b2));
			var value3 = sp.FindPropertyRelative(nameof(ACTkByte16.b3));
			var value4 = sp.FindPropertyRelative(nameof(ACTkByte16.b4));
			var value5 = sp.FindPropertyRelative(nameof(ACTkByte16.b5));
			var value6 = sp.FindPropertyRelative(nameof(ACTkByte16.b6));
			var value7 = sp.FindPropertyRelative(nameof(ACTkByte16.b7));
			var value8 = sp.FindPropertyRelative(nameof(ACTkByte16.b8));
			var value9 = sp.FindPropertyRelative(nameof(ACTkByte16.b9));
			var value10 = sp.FindPropertyRelative(nameof(ACTkByte16.b10));
			var value11 = sp.FindPropertyRelative(nameof(ACTkByte16.b11));
			var value12 = sp.FindPropertyRelative(nameof(ACTkByte16.b12));
			var value13 = sp.FindPropertyRelative(nameof(ACTkByte16.b13));
			var value14 = sp.FindPropertyRelative(nameof(ACTkByte16.b14));
			var value15 = sp.FindPropertyRelative(nameof(ACTkByte16.b15));
			var value16 = sp.FindPropertyRelative(nameof(ACTkByte16.b16));

			var union = new DecimalLongBytesUnion();
			
			union.b16.b1 = (byte)value1.intValue;
			union.b16.b2 = (byte)value2.intValue;
			union.b16.b3 = (byte)value3.intValue;
			union.b16.b4 = (byte)value4.intValue;
			union.b16.b5 = (byte)value5.intValue;
			union.b16.b6 = (byte)value6.intValue;
			union.b16.b7 = (byte)value7.intValue;
			union.b16.b8 = (byte)value8.intValue;
			union.b16.b9 = (byte)value9.intValue;
			union.b16.b10 = (byte)value10.intValue;
			union.b16.b11 = (byte)value11.intValue;
			union.b16.b12 = (byte)value12.intValue;
			union.b16.b13 = (byte)value13.intValue;
			union.b16.b14 = (byte)value14.intValue;
			union.b16.b15 = (byte)value15.intValue;
			union.b16.b16 = (byte)value16.intValue;

			return union;
		}
		
		private void WriteACTkByte16(SerializedProperty sp, DecimalLongBytesUnion union)
		{
			var value1 = sp.FindPropertyRelative(nameof(ACTkByte16.b1));
			var value2 = sp.FindPropertyRelative(nameof(ACTkByte16.b2));
			var value3 = sp.FindPropertyRelative(nameof(ACTkByte16.b3));
			var value4 = sp.FindPropertyRelative(nameof(ACTkByte16.b4));
			var value5 = sp.FindPropertyRelative(nameof(ACTkByte16.b5));
			var value6 = sp.FindPropertyRelative(nameof(ACTkByte16.b6));
			var value7 = sp.FindPropertyRelative(nameof(ACTkByte16.b7));
			var value8 = sp.FindPropertyRelative(nameof(ACTkByte16.b8));
			var value9 = sp.FindPropertyRelative(nameof(ACTkByte16.b9));
			var value10 = sp.FindPropertyRelative(nameof(ACTkByte16.b10));
			var value11 = sp.FindPropertyRelative(nameof(ACTkByte16.b11));
			var value12 = sp.FindPropertyRelative(nameof(ACTkByte16.b12));
			var value13 = sp.FindPropertyRelative(nameof(ACTkByte16.b13));
			var value14 = sp.FindPropertyRelative(nameof(ACTkByte16.b14));
			var value15 = sp.FindPropertyRelative(nameof(ACTkByte16.b15));
			var value16 = sp.FindPropertyRelative(nameof(ACTkByte16.b16));

			value1.intValue = union.b16.b1;
			value2.intValue = union.b16.b2;
			value3.intValue = union.b16.b3;
			value4.intValue = union.b16.b4;
			value5.intValue = union.b16.b5;
			value6.intValue = union.b16.b6;
			value7.intValue = union.b16.b7;
			value8.intValue = union.b16.b8;
			value9.intValue = union.b16.b9;
			value10.intValue = union.b16.b10;
			value11.intValue = union.b16.b11;
			value12.intValue = union.b16.b12;
			value13.intValue = union.b16.b13;
			value14.intValue = union.b16.b14;
			value15.intValue = union.b16.b15;
			value16.intValue = union.b16.b16;
		}
	}
}

#endif