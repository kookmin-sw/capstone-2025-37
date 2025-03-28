#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredQuaternion
	{
		/// <summary>
		/// Used to store encrypted Quaternion.
		/// </summary>
		[Serializable]
		public struct RawEncryptedQuaternion : IEquatable<RawEncryptedQuaternion>
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

			/// <summary>
			/// Encrypted value
			/// </summary>
			public int w;

			public bool Equals(RawEncryptedQuaternion other)
			{
				return x == other.x && y == other.y && z == other.z && w == other.w;
			}

			public override bool Equals(object obj)
			{
				return obj is RawEncryptedQuaternion other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = x;
					hashCode = (hashCode * 397) ^ y;
					hashCode = (hashCode * 397) ^ z;
					hashCode = (hashCode * 397) ^ w;
					return hashCode;
				}
			}
		}
	}
}