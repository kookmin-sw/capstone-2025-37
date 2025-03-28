#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredInt : IFormattable, IEquatable<ObscuredInt>, IEquatable<int>, IComparable<ObscuredInt>, IComparable<int>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredInt(int value)
		{
			return new ObscuredInt(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator int(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredFloat(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredDouble(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static explicit operator ObscuredUInt(ObscuredInt value)
		{
			return (uint)value.InternalDecrypt();
		}

		public static ObscuredInt operator ++(ObscuredInt input)
		{
			return Increment(input, 1);
		}

		public static ObscuredInt operator --(ObscuredInt input)
		{
			return Increment(input, -1);
		}

		private static ObscuredInt Increment(ObscuredInt input, int increment)
		{
			var decrypted = input.InternalDecrypt() + increment;
			input.HideValue(decrypted);
			
			if (ObscuredCheatingDetector.IsRunningInHoneypotMode)
				input.fakeValue = decrypted;

			return input;
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public override bool Equals(object other)
		{
			return other is ObscuredInt o && Equals(o) ||
				   other is int r && Equals(r);
		}
		
		public bool Equals(ObscuredInt other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(int other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredInt other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(int other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}