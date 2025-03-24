#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using System.Numerics;
using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredBigInteger : SerializedObscuredType<BigInteger>
	{
		public BigInteger Hidden
		{
			get => GetBigInteger(HiddenProperty);
			set => SetBigInteger(HiddenProperty, value);
		}

		public uint Key
		{
			get => (uint)KeyProperty.intValue;
			set => KeyProperty.intValue = (int)value;
		}

		public override bool IsDataValid => HashUtils.CalculateHash(Plain) == Hash;
		public override BigInteger Plain => ObscuredBigInteger.Decrypt(Hidden, Key);
		protected override byte TypeVersion => ObscuredBigInteger.Version;

		private static BigInteger GetBigInteger(SerializedProperty serializableBigInteger)
		{
			var result = new SerializableBigInteger();
			var rawProperty = serializableBigInteger.FindPropertyRelative(nameof(SerializableBigInteger.raw));
			var signProperty = rawProperty.FindPropertyRelative(nameof(SerializableBigInteger.BigIntegerContents.sign));
			var bitsProperty = rawProperty.FindPropertyRelative(nameof(SerializableBigInteger.BigIntegerContents.bits));
			var bits = ReadBitsArray(bitsProperty);
			
			result.raw = new SerializableBigInteger.BigIntegerContents
			{
				sign = signProperty.intValue,
				bits = bits
			};

			return result.value;
		}
		
		private static void SetBigInteger(SerializedProperty serializableBigInteger, BigInteger value)
		{
			var explicitStruct = new SerializableBigInteger
			{
				value = value
			};

			var sign = explicitStruct.raw.sign;
			var bits = explicitStruct.raw.bits;
			
			var rawProperty = serializableBigInteger.FindPropertyRelative(nameof(SerializableBigInteger.raw));
			var signProperty = rawProperty.FindPropertyRelative(nameof(SerializableBigInteger.BigIntegerContents.sign));
			var bitsProperty = rawProperty.FindPropertyRelative(nameof(SerializableBigInteger.BigIntegerContents.bits));

			signProperty.intValue = sign;
			WriteBitsArray(bitsProperty, bits);
		}
		
		private static uint[] ReadBitsArray(SerializedProperty bits)
		{
			var count = bits.arraySize;
			if (count == 0)
				return null;
			var result = new uint[count];
			for (var i = 0; i < count; i++)
				result[i] = (uint)bits.GetArrayElementAtIndex(i).longValue;

			return result;
		}

		private static void WriteBitsArray(SerializedProperty bitsProperty, uint[] bits)
		{
			bitsProperty.arraySize = bits?.Length ?? 0;
			for (var i = 0; i < bits?.Length; i++)
				bitsProperty.GetArrayElementAtIndex(i).longValue = bits[i];
		}
	}
}

#endif