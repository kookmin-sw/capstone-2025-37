#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredDateTime : IFormattable, IEquatable<ObscuredDateTime>, IEquatable<DateTime>, IComparable<ObscuredDateTime>, IComparable<DateTime>, IComparable, IConvertible
	{
		/// <summary>Gets the number of ticks that represent the date and time of this instance.</summary>
		/// <returns>The number of ticks that represent the date and time of this instance. The value is between <see langword="DateTime.MinValue.Ticks" /> and <see langword="DateTime.MaxValue.Ticks" />.</returns>
		public long Ticks => GetDecrypted().Ticks;
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredDateTime(DateTime value)
		{
			return new ObscuredDateTime(value);
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator DateTime(ObscuredDateTime value)
		{
			return value.InternalDecryptAsDateTime();
		}

		public override int GetHashCode()
		{
			return InternalDecryptAsDateTime().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecryptAsDateTime().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecryptAsDateTime().ToString(format);
		}

		public TypeCode GetTypeCode() {
            return TypeCode.DateTime;
        }
 
 
        bool IConvertible.ToBoolean(IFormatProvider provider) => throw new InvalidCastException();
        char IConvertible.ToChar(IFormatProvider provider) => throw new InvalidCastException();
        sbyte IConvertible.ToSByte(IFormatProvider provider) => throw new InvalidCastException();
        byte IConvertible.ToByte(IFormatProvider provider) => throw new InvalidCastException();
        short IConvertible.ToInt16(IFormatProvider provider) => throw new InvalidCastException();
        ushort IConvertible.ToUInt16(IFormatProvider provider) => throw new InvalidCastException();
        int IConvertible.ToInt32(IFormatProvider provider) => throw new InvalidCastException();
        uint IConvertible.ToUInt32(IFormatProvider provider) => throw new InvalidCastException();
        long IConvertible.ToInt64(IFormatProvider provider) => throw new InvalidCastException();
        ulong IConvertible.ToUInt64(IFormatProvider provider) => throw new InvalidCastException();
        float IConvertible.ToSingle(IFormatProvider provider) => throw new InvalidCastException();
        double IConvertible.ToDouble(IFormatProvider provider) => throw new InvalidCastException();
        decimal IConvertible.ToDecimal(IFormatProvider provider) => throw new InvalidCastException();
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => this;
        object IConvertible.ToType(Type type, IFormatProvider provider) 
		{
			if (type == typeof(DateTime))
				return InternalDecryptAsDateTime();
			
			if (type == typeof(ObscuredDateTime))
				return this;
			
            return Convert.ChangeType(InternalDecryptAsDateTime(), type, provider);
        }

		public string ToString(IFormatProvider provider)
		{
			return InternalDecryptAsDateTime().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecryptAsDateTime().ToString(format, provider);
		}

		public override bool Equals(object other)
		{
			return other is ObscuredDateTime o && Equals(o) ||
				   other is DateTime r && Equals(r);
		}

		public bool Equals(ObscuredDateTime other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(DateTime other)
		{
			return InternalDecryptAsDateTime().Equals(other);
		}

		public int CompareTo(ObscuredDateTime other)
		{
			return InternalDecryptAsDateTime().CompareTo(other.InternalDecryptAsDateTime());
		}

		public int CompareTo(DateTime other)
		{
			return InternalDecryptAsDateTime().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecryptAsDateTime().CompareTo(obj);
		}

		public static DateTime operator +(ObscuredDateTime d, TimeSpan t)
		{
			return (DateTime)d + t;
		}
		
		public static DateTime operator -(ObscuredDateTime d, TimeSpan t)
		{
			return (DateTime)d - t;
		}

		public static TimeSpan operator -(ObscuredDateTime d1, DateTime d2)
		{
			return (DateTime)d1 - d2;
		}
		
		public static TimeSpan operator -(DateTime d1, ObscuredDateTime d2)
		{
			return d1 - (DateTime)d2;
		}

		public static bool operator ==(ObscuredDateTime d1, DateTime d2)
		{
			return (DateTime)d1 == d2;
		}
		
		public static bool operator ==(DateTime d1, ObscuredDateTime d2)
		{
			return d1 == (DateTime)d2;
		}

		public static bool operator !=(ObscuredDateTime d1, DateTime d2)
		{
			return (DateTime)d1 != d2;
		}
		
		public static bool operator !=(DateTime d1, ObscuredDateTime d2)
		{
			return d1 != (DateTime)d2;
		}

		public static bool operator <(ObscuredDateTime t1, DateTime t2)
		{
			return (DateTime)t1 < t2;
		}
		
		public static bool operator <(DateTime t1, ObscuredDateTime t2)
		{
			return t1 < (DateTime)t2;
		}
		
		public static bool operator <=(ObscuredDateTime t1, DateTime t2)
		{
			return (DateTime)t1 <= t2;
		}
		
		public static bool operator <=(DateTime t1, ObscuredDateTime t2)
		{
			return t1 <= (DateTime)t2;
		}

		public static bool operator >(ObscuredDateTime t1, DateTime t2)
		{
			return (DateTime)t1 > t2;
		}
		
		public static bool operator >(DateTime t1, ObscuredDateTime t2)
		{
			return t1 > (DateTime)t2;
		}
		
		public static bool operator >=(ObscuredDateTime t1, DateTime t2)
		{
			return (DateTime)t1 >= t2;
		}
		
		public static bool operator >=(DateTime t1, ObscuredDateTime t2)
		{
			return t1 >= (DateTime)t2;
		}
	}
}