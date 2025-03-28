#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Reflection;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredQuaternion : IEquatable<ObscuredQuaternion>, IEquatable<Quaternion>
	{
		[Obfuscation(Exclude = true)]
		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			return new ObscuredQuaternion(value);
		}

		[Obfuscation(Exclude = true)]
		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}
		
		public static ObscuredQuaternion operator *(ObscuredQuaternion lhs, ObscuredQuaternion rhs)
		{
			return new ObscuredQuaternion((Quaternion)lhs * (Quaternion)rhs);
		}
		
		public static ObscuredQuaternion operator *(ObscuredQuaternion lhs, Quaternion rhs)
		{
			return new ObscuredQuaternion((Quaternion)lhs * rhs);
		}
		
		public override bool Equals(object other)
		{
			return other is ObscuredQuaternion o && Equals(o) ||
				   other is Quaternion r && Equals(r);
		}

		public bool Equals(ObscuredQuaternion other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}

		public bool Equals(Quaternion other)
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
		
		public Quaternion normalized => InternalDecrypt().normalized;
	}
}