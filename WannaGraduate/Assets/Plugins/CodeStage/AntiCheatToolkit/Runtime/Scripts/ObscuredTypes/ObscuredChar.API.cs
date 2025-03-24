#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Detectors;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredChar : IEquatable<ObscuredChar>, IEquatable<char>, IComparable<ObscuredChar>, IComparable<char>, IComparable
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredChar(char value)
		{
			return new ObscuredChar(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredChar operator ++(ObscuredChar input)
		{
			return Increment(input, 1);
		}

		public static ObscuredChar operator --(ObscuredChar input)
		{
			return Increment(input, -1);
		}

		private static ObscuredChar Increment(ObscuredChar input, int increment)
		{
			var decrypted = (char)(input.InternalDecrypt() + increment);
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

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public override bool Equals(object other)
		{
			return other is ObscuredChar o && Equals(o) ||
				   other is char r && Equals(r);
		}
		
		public bool Equals(ObscuredChar other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}
		
		public bool Equals(char other)
		{
			return InternalDecrypt().Equals(other);
		}

		public int CompareTo(ObscuredChar other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(char other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			return InternalDecrypt().CompareTo(obj);
		}
	}
}