#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredByte : IFormattable, IEquatable<ObscuredByte>, IEquatable<byte>, IComparable<ObscuredByte>, IComparable<byte>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredByte(byte value)
		{
			return new ObscuredByte(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator byte(ObscuredByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredByte operator ++(ObscuredByte input)
		{
			return Increment(input, 1);
		}

		public static ObscuredByte operator --(ObscuredByte input)
		{
			return Increment(input, -1);
		}

		private static ObscuredByte Increment(ObscuredByte input, int increment)
		{
			var decrypted = (byte)(input.InternalDecrypt() + increment);
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
			return other is ObscuredByte o && Equals(o) ||
				   other is byte r && Equals(r);
		}

		public bool Equals(ObscuredByte obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public bool Equals(byte other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredByte other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(byte other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}