#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredFloat : IFormattable, IEquatable<ObscuredFloat>, IEquatable<float>, IComparable<ObscuredFloat>, IComparable<float>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredFloat(float value)
		{
			return new ObscuredFloat(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator float(ObscuredFloat value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredFloat operator ++(ObscuredFloat input)
		{
			return Increment(input, 1);
		}

		public static ObscuredFloat operator --(ObscuredFloat input)
		{
			return Increment(input, -1);
		}

		private static ObscuredFloat Increment(ObscuredFloat input, int increment)
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
			return other is ObscuredFloat o && Equals(o) ||
				   other is float r && Equals(r);
		}
		
		public bool Equals(ObscuredFloat other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(float other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredFloat other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(float other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}