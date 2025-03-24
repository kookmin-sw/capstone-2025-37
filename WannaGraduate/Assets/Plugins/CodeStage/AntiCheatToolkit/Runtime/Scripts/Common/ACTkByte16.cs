#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Common
{
	using System;

	[Serializable]
	internal struct ACTkByte16 : IEquatable<ACTkByte16>
	{
		public byte b1;
		public byte b2;
		public byte b3;
		public byte b4;
		public byte b5;
		public byte b6;
		public byte b7;
		public byte b8;
		public byte b9;
		public byte b10;
		public byte b11;
		public byte b12;
		public byte b13;
		public byte b14;
		public byte b15;
		public byte b16;

		public bool Equals(ACTkByte16 other)
		{
			return b1 == other.b1 && b2 == other.b2 && b3 == other.b3 && b4 == other.b4 && b5 == other.b5 && b6 == other.b6 && b7 == other.b7 && b8 == other.b8 && b9 == other.b9 && b10 == other.b10 && b11 == other.b11 && b12 == other.b12 && b13 == other.b13 && b14 == other.b14 && b15 == other.b15 && b16 == other.b16;
		}

		public override bool Equals(object obj)
		{
			return obj is ACTkByte16 other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = b1.GetHashCode();
				hashCode = (hashCode * 397) ^ b2.GetHashCode();
				hashCode = (hashCode * 397) ^ b3.GetHashCode();
				hashCode = (hashCode * 397) ^ b4.GetHashCode();
				hashCode = (hashCode * 397) ^ b5.GetHashCode();
				hashCode = (hashCode * 397) ^ b6.GetHashCode();
				hashCode = (hashCode * 397) ^ b7.GetHashCode();
				hashCode = (hashCode * 397) ^ b8.GetHashCode();
				hashCode = (hashCode * 397) ^ b9.GetHashCode();
				hashCode = (hashCode * 397) ^ b10.GetHashCode();
				hashCode = (hashCode * 397) ^ b11.GetHashCode();
				hashCode = (hashCode * 397) ^ b12.GetHashCode();
				hashCode = (hashCode * 397) ^ b13.GetHashCode();
				hashCode = (hashCode * 397) ^ b14.GetHashCode();
				hashCode = (hashCode * 397) ^ b15.GetHashCode();
				hashCode = (hashCode * 397) ^ b16.GetHashCode();
				return hashCode;
			}
		}
	}
}