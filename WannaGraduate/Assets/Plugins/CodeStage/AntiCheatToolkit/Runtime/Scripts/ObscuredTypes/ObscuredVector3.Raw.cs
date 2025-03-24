#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector3
	{
		/// <summary>
		/// Used to store encrypted Vector3.
		/// </summary>
		[Serializable]
		public struct RawEncryptedVector3 : IEquatable<RawEncryptedVector3>
		{
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int x;
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int y;
			/// <summary>
			/// Encrypted value
			/// </summary>
			public int z;

			public bool Equals(RawEncryptedVector3 other)
			{
				return x == other.x && y == other.y && z == other.z;
			}

			public override bool Equals(object obj)
			{
				return obj is RawEncryptedVector3 other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = x;
					hashCode = (hashCode * 397) ^ y;
					hashCode = (hashCode * 397) ^ z;
					return hashCode;
				}
			}
		}
	}
}