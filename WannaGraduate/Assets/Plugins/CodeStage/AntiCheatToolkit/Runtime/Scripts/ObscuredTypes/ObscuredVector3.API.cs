#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector3 : IEquatable<ObscuredVector3>, IEquatable<Vector3>
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredVector3(Vector3 value)
		{
			return new ObscuredVector3(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(Vector3 a, ObscuredVector3 b)
		{
			return a + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() + b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(Vector3 a, ObscuredVector3 b)
		{
			return a - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() - b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a)
		{
			return -a.InternalDecrypt();
		}

		public static ObscuredVector3 operator *(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() * d;
		}

		public static ObscuredVector3 operator *(float d, ObscuredVector3 a)
		{
			return d * a.InternalDecrypt();
		}

		public static ObscuredVector3 operator /(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		public static bool operator ==(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}

		public static bool operator ==(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}

		public static bool operator ==(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}

		public static bool operator !=(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}

		public static bool operator !=(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}

		public override bool Equals(object other)
		{
			return other is ObscuredVector3 o && Equals(o) ||
				   other is Vector3 r && Equals(r);
		}

		public bool Equals(ObscuredVector3 other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}

		public bool Equals(Vector3 other)
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

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}
		
		public void Normalize()
		{
			var temp = InternalDecrypt();
			temp.Normalize();
			SetEncrypted(Encrypt(normalized, currentCryptoKey), currentCryptoKey);
		}
		
		public Vector3 normalized => InternalDecrypt().normalized;
	}
}