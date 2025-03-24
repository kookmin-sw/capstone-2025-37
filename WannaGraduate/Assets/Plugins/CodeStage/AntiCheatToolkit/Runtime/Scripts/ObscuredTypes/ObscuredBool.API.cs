#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredBool : IEquatable<ObscuredBool>, IEquatable<bool>, IComparable<ObscuredBool>, IComparable<bool>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredBool(bool value)
		{
			return new ObscuredBool(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator bool(ObscuredBool value)
		{
			return value.InternalDecrypt();
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public override bool Equals(object other)
		{
			return other is ObscuredBool o && Equals(o) ||
				   other is bool r && Equals(r);
		}
		
		public bool Equals(ObscuredBool obj)
		{
			return currentCryptoKey == obj.currentCryptoKey ? hiddenValue.Equals(obj.hiddenValue) : 
				InternalDecrypt().Equals(obj.InternalDecrypt());
		}
		
		public bool Equals(bool other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredBool other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(bool other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}