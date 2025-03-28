#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public sealed partial class ObscuredString : IEquatable<ObscuredString>, IEquatable<string>, IComparable<ObscuredString>, IComparable<string>, IComparable
	{
		public int Length => hiddenChars.Length;

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
					throw new IndexOutOfRangeException();
				return InternalDecrypt()[index];
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredString(string value)
		{
			return value == null ? null : new ObscuredString(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator string(ObscuredString value)
		{
			return value == null ? null : value.InternalDecryptToString();
		}

		/// <summary>
		/// Determines whether two specified ObscuredStrings have the same value.
		/// </summary>
		///
		/// <returns>
		/// true if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, false.
		/// </returns>
		/// <param name="a">An ObscuredString or null. </param><param name="b">An ObscuredString or null. </param>
		public static bool operator ==(ObscuredString a, ObscuredString b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if ((object)a == null || (object)b == null)
				return false;

			return a.cryptoKey == b.cryptoKey
				? ArraysEquals(a.hiddenChars, b.hiddenChars)
				: ArraysEquals(a.InternalDecrypt(), b.InternalDecrypt());
		}

		/// <summary>
		/// Determines whether two specified ObscuredStrings have different values.
		/// </summary>
		///
		/// <returns>
		/// true if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>; otherwise, false.
		/// </returns>
		/// <param name="a">An ObscuredString or null. </param><param name="b">An ObscuredString or null. </param>
		public static bool operator !=(ObscuredString a, ObscuredString b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public string Substring(int startIndex)
		{
			return Substring(startIndex, Length - startIndex);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public string Substring(int startIndex, int length)
		{
			return InternalDecryptToString().Substring(startIndex, length);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public bool StartsWith(string value, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			return InternalDecryptToString().StartsWith(value, comparisonType);
		}

		/// <summary>
		/// Proxy to the String API.
		/// Please consider avoiding using this in a hot path since it invokes decryption on every access call.
		/// </summary>
		public bool EndsWith(string value, StringComparison comparisonType = StringComparison.CurrentCulture)
		{
			return InternalDecryptToString().EndsWith(value, comparisonType);
		}

		public override int GetHashCode()
		{
			return InternalDecryptToString().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecryptToString();
		}
		
		public override bool Equals(object other)
		{
			return other is ObscuredString o && Equals(o) ||
				   other is string r && Equals(r);
		}
		
		public bool Equals(ObscuredString obj)
		{
			if (obj == null) return false;
			
			return cryptoKey == obj.cryptoKey ? ArraysEquals(hiddenChars, obj.hiddenChars) : 
				ArraysEquals(InternalDecrypt(), obj.InternalDecrypt());
		}

		public bool Equals(ObscuredString value, StringComparison comparisonType)
		{
			return value != null &&
				   string.Equals(InternalDecryptToString(), value.InternalDecryptToString(), comparisonType);
		}
		
		public bool Equals(string other)
		{
			return InternalDecryptToString().Equals(other);
		}
		
		public int CompareTo(ObscuredString other)
		{
			return InternalDecryptToString().CompareTo(other.InternalDecryptToString());
		}

		public int CompareTo(string other)
		{
			return InternalDecryptToString().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecryptToString().CompareTo(obj);
		}
	}
}