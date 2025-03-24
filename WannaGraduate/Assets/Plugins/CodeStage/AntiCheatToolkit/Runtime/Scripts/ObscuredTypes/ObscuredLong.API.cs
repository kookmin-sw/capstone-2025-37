#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredLong : IFormattable, IEquatable<ObscuredLong>, IEquatable<long>, IComparable<ObscuredLong>, IComparable<long>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredLong(long value)
		{
			return new ObscuredLong(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator long(ObscuredLong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredLong operator ++(ObscuredLong input)
		{
			return Increment(input, 1);
		}

		public static ObscuredLong operator --(ObscuredLong input)
		{
			return Increment(input, -1);
		}

		private static ObscuredLong Increment(ObscuredLong input, int increment)
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
			return other is ObscuredLong o && Equals(o) ||
				   other is long r && Equals(r);
		}

		public bool Equals(ObscuredLong other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(long other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredLong other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(long other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}
