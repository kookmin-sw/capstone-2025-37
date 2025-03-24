#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector2Int : IEquatable<ObscuredVector2Int>, IEquatable<Vector2Int>
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredVector2Int(Vector2Int value)
		{
			return new ObscuredVector2Int(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector2Int(ObscuredVector2Int value)
		{
			return value.InternalDecrypt();
		}
		
		[System.Reflection.Obfuscation(Exclude = true)]
		public static explicit operator Vector3Int(ObscuredVector2Int v)
		{
			return new Vector3Int(v.x, v.y, 0);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector2(ObscuredVector2Int value)
		{
			return value.InternalDecrypt();
		}
		
		public static ObscuredVector2Int operator -(ObscuredVector2Int v)
		{
			return new ObscuredVector2Int(-v.x, -v.y);
		}
		
		public static ObscuredVector2Int operator +(ObscuredVector2Int a,  ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x + b.x, a.y + b.y);
		}
		
		public static ObscuredVector2Int operator +(Vector2Int a,  ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x + b.x, a.y + b.y);
		}
		
		public static ObscuredVector2Int operator -(ObscuredVector2Int a, ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x - b.x, a.y - b.y);
		}
		
		public static ObscuredVector2Int operator -(ObscuredVector2Int a, Vector2Int b)
		{
			return new ObscuredVector2Int(a.x - b.x, a.y - b.y);
		}

		public static ObscuredVector2Int operator +(ObscuredVector2Int a, Vector2Int b)
		{
			return new ObscuredVector2Int(a.x + b.x, a.y + b.y);
		}
		
		public static ObscuredVector2Int operator -(Vector2Int a, ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x - b.x, a.y - b.y);
		}
		
		public static ObscuredVector2Int operator *(ObscuredVector2Int a, ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x * b.x, a.y * b.y);
		}
		
		public static ObscuredVector2Int operator *(Vector2Int a, ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a.x * b.x, a.y * b.y);
		}
		
		public static ObscuredVector2Int operator *(ObscuredVector2Int a, Vector2Int b)
		{
			return new ObscuredVector2Int(a.x * b.x, a.y * b.y);
		}
		
		public static ObscuredVector2Int operator *(int a, ObscuredVector2Int b)
		{
			return new ObscuredVector2Int(a * b.x, a * b.y);
		}

		public static ObscuredVector2Int operator *(ObscuredVector2Int a, int b)
		{
			return new ObscuredVector2Int(a.x * b, a.y * b);
		}

		public static ObscuredVector2Int operator /(ObscuredVector2Int a, int b)
		{
			return new ObscuredVector2Int(a.x / b, a.y / b);
		}

		public static bool operator ==(ObscuredVector2Int lhs, ObscuredVector2Int rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}
		
		public static bool operator !=(ObscuredVector2Int lhs, ObscuredVector2Int rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			return other is ObscuredVector2Int o && Equals(o) ||
				   other is Vector2Int r && Equals(r);
		}

		public bool Equals(ObscuredVector2Int other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}

		public bool Equals(Vector2Int other)
		{
			return InternalDecrypt().Equals(other);
		}	

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}
	}
}