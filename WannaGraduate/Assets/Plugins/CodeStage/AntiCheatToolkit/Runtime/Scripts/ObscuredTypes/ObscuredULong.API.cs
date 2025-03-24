#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredULong : IFormattable, IEquatable<ObscuredULong>, IEquatable<ulong>, IComparable<ObscuredULong>, IComparable<ulong>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredULong(ulong value)
		{
			return new ObscuredULong(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ulong(ObscuredULong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredULong operator ++(ObscuredULong input)
		{
			return Increment(input, true);
		}

		public static ObscuredULong operator --(ObscuredULong input)
		{
			return Increment(input, false);
		}

		private static ObscuredULong Increment(ObscuredULong input, bool positive)
		{
			var decrypted = input.InternalDecrypt();
			if (positive)
				decrypted++;
			else
				decrypted--;
			
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
			return other is ObscuredULong o && Equals(o) ||
				   other is ulong r && Equals(r);
		}

		public bool Equals(ObscuredULong obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public bool Equals(ulong other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredULong other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(ulong other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}