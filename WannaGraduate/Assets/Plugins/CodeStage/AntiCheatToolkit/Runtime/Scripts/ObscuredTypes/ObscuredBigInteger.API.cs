#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Numerics;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredBigInteger : IFormattable, IEquatable<ObscuredBigInteger>, IEquatable<BigInteger>, IComparable<ObscuredBigInteger>, IComparable<BigInteger>, IComparable
	{
		//! @cond
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(BigInteger value)
		{
			return new ObscuredBigInteger(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(byte value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(sbyte value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(short value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(ushort value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(int value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(uint value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(long value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(ulong value)
		{
			return new ObscuredBigInteger(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(float value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(double value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBigInteger(decimal value)
		{
			return new ObscuredBigInteger((BigInteger)value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator BigInteger(ObscuredBigInteger value)
		{
			return value.InternalDecrypt();
		}
		
		public static explicit operator byte(ObscuredBigInteger value)
		{
			return (byte)value.InternalDecrypt();
		}
		
		public static explicit operator sbyte(ObscuredBigInteger value)
		{
			return (sbyte)value.InternalDecrypt();
		}
		
		public static explicit operator short(ObscuredBigInteger value)
		{
			return (short)value.InternalDecrypt();
		}
		
		public static explicit operator ushort(ObscuredBigInteger value)
		{
			return (ushort)value.InternalDecrypt();
		}
		
		public static explicit operator int(ObscuredBigInteger value)
		{
			return (int)value.InternalDecrypt();
		}
		
		public static explicit operator uint(ObscuredBigInteger value)
		{
			return (uint)value.InternalDecrypt();
		}
		
		public static explicit operator long(ObscuredBigInteger value)
		{
			return (long)value.InternalDecrypt();
		}
		
		public static explicit operator ulong(ObscuredBigInteger value)
		{
			return (ulong)value.InternalDecrypt();
		}
		
		public static explicit operator float(ObscuredBigInteger value)
		{
			return (float)value.InternalDecrypt();
		}
		
		public static explicit operator double(ObscuredBigInteger value)
		{
			return (double)value.InternalDecrypt();
		}
		
		public static explicit operator decimal(ObscuredBigInteger value)
		{
			return (decimal)value.InternalDecrypt();
		}

		public static ObscuredBigInteger operator <<(ObscuredBigInteger value, int shift)
		{
			return (BigInteger)value << shift;
		}
		
		public static ObscuredBigInteger operator >>(ObscuredBigInteger value, int shift)
		{
			return (BigInteger)value >> shift;
		}
		
		public static ObscuredBigInteger operator ~(ObscuredBigInteger value)
		{
			return ~(BigInteger)value;
		}
		
		public static ObscuredBigInteger operator +(ObscuredBigInteger value)
		{
			return +(BigInteger)value;
		}
		
		public static ObscuredBigInteger operator -(ObscuredBigInteger value)
		{
			return -(BigInteger)value;
		}
		
		public static ObscuredBigInteger operator +(ObscuredBigInteger left, long right)
		{
			return (long)left + right;
		}
		
		public static ObscuredBigInteger operator -(ObscuredBigInteger left, long right)
		{
			return (long)left - right;
		}
		
		public static ObscuredBigInteger operator ++(ObscuredBigInteger input)
		{
			return Increment(input, 1);
		}
		
		public static ObscuredBigInteger operator --(ObscuredBigInteger input)
		{
			return Increment(input, -1);
		}

		public static bool operator <(ObscuredBigInteger left, long right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(ObscuredBigInteger left, long right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left > right;
		}
		
		public static bool operator >=(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left >= right;
		}
		
		public static bool operator ==(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left == right;
		}
		
		public static bool operator !=(ObscuredBigInteger left, long right)
		{
			return (BigInteger)left != right;
		}
		
		public static bool operator <(ObscuredBigInteger left, ulong right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(ObscuredBigInteger left, ulong right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left > right;
		}
		
		public static bool operator >=(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left >= right;
		}
		
		public static bool operator ==(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left == right;
		}
		
		public static bool operator !=(ObscuredBigInteger left, ulong right)
		{
			return (BigInteger)left != right;
		}
		
		public static bool operator <(long left, ObscuredBigInteger right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(long left, ObscuredBigInteger right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(long left, ObscuredBigInteger right)
		{
			return left > (BigInteger)right;
		}
		
		public static bool operator >=(long left, ObscuredBigInteger right)
		{
			return left >= (BigInteger)right;
		}
		
		public static bool operator ==(long left, ObscuredBigInteger right)
		{
			return left == (BigInteger)right;
		}
		
		public static bool operator !=(long left, ObscuredBigInteger right)
		{
			return left != (BigInteger)right;
		}
		
		public static bool operator <(ulong left, ObscuredBigInteger right)
		{
			return left < (BigInteger)right;
		}
		
		public static bool operator <=(ulong left, ObscuredBigInteger right)
		{
			return left <= (BigInteger)right;
		}
		
		public static bool operator >(ulong left, ObscuredBigInteger right)
		{
			return left > (BigInteger)right;
		}
		
		public static bool operator >=(ulong left, ObscuredBigInteger right)
		{
			return left >= (BigInteger)right;
		}
		
		public static bool operator ==(ulong left, ObscuredBigInteger right)
		{
			return left == (BigInteger)right;
		}
		
		public static bool operator !=(ulong left, ObscuredBigInteger right)
		{
			return left != (BigInteger)right;
		}

		public static ObscuredBigInteger operator &(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left & (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator |(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left | (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator ^(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left ^ (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator +(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left + (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator -(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left - (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator *(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left * (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator /(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left / (BigInteger)right;
		}
		
		public static ObscuredBigInteger operator %(ObscuredBigInteger left, ObscuredBigInteger right)
		{
			return (BigInteger)left % (BigInteger)right;
		}

		private static ObscuredBigInteger Increment(ObscuredBigInteger input, int increment)
		{
			var decrypted = (BigInteger)((int)input.InternalDecrypt() + increment);
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
			return InternalDecrypt().ToString(format, provider) ?? string.Empty;
		}

		public override bool Equals(object other)
		{
			return other is ObscuredBigInteger o && Equals(o) ||
				   other is BigInteger r && Equals(r);
		}

		public bool Equals(BigInteger other)
		{
			return InternalDecrypt().Equals(other);
		}
		
		public bool Equals(ObscuredBigInteger obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public int CompareTo(ObscuredBigInteger other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(BigInteger other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}

		public int CompareTo(long other)
		{
			return InternalDecrypt().CompareTo(other);
		}		
		
		public int CompareTo(ulong other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public byte[] ToByteArray()
		{
			return InternalDecrypt().ToByteArray();
		}
		
		//! @endcond
	}
}
