#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector3Int
	{
		/// <summary>
		/// Used to store encrypted Vector3Int.
		/// </summary>
		[Serializable]
		public struct RawEncryptedVector3Int : IEquatable<RawEncryptedVector3Int>
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

			public bool Equals(RawEncryptedVector3Int other)
			{
				return x == other.x && y == other.y && z == other.z;
			}

			public override bool Equals(object obj)
			{
				return obj is RawEncryptedVector3Int other && Equals(other);
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