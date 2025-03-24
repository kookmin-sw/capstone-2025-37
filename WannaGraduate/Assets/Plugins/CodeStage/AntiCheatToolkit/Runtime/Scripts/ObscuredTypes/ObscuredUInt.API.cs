#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredUInt : IFormattable, IEquatable<ObscuredUInt>, IEquatable<uint>, IComparable<ObscuredUInt>, IComparable<uint>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredUInt(uint value)
		{
			return new ObscuredUInt(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator uint(ObscuredUInt value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredInt(ObscuredUInt value)
		{
			return (int)value.InternalDecrypt();
		}

		public static ObscuredUInt operator ++(ObscuredUInt input)
		{
			return Increment(input, 1);
		}

		public static ObscuredUInt operator --(ObscuredUInt input)
		{
			return Increment(input, -1);
		}

		private static ObscuredUInt Increment(ObscuredUInt input, int increment)
		{
			var decrypted = (uint)(input.InternalDecrypt() + increment);
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
			return other is ObscuredUInt o && Equals(o) ||
				   other is uint r && Equals(r);
		}

		public bool Equals(ObscuredUInt obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public bool Equals(uint other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredUInt other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(uint other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}