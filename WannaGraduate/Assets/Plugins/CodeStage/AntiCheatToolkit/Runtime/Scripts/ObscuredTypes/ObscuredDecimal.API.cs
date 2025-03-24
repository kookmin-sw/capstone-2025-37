#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredDecimal : IFormattable, IEquatable<ObscuredDecimal>, IEquatable<decimal>, IComparable<ObscuredDecimal>, IComparable<decimal>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredDecimal(decimal value)
		{
			return new ObscuredDecimal(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredDecimal(ObscuredFloat f)
		{
			return (decimal)(float)f;
		}

		public static ObscuredDecimal operator ++(ObscuredDecimal input)
		{
			return Increment(input, 1);
		}

		public static ObscuredDecimal operator --(ObscuredDecimal input)
		{
			return Increment(input, -1);
		}

		private static ObscuredDecimal Increment(ObscuredDecimal input, decimal increment)
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
			return other is ObscuredDecimal o && Equals(o) ||
				   other is decimal r && Equals(r);
		}
		
		public bool Equals(ObscuredDecimal other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(decimal other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredDecimal other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(decimal other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}