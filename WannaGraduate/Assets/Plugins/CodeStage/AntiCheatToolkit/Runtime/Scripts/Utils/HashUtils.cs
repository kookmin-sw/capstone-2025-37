#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
#if UNITY_2021_2_OR_NEWER
using System.Buffers;
#endif
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CodeStage.AntiCheat.Utils
{
	internal static class HashUtils
	{
		private const int Salt = 0x7E3779B9;

		public static bool ValidateHash(BigInteger input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(bool input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(decimal input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(int input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(long input, int hash)
		{
			return CalculateHash(input) == hash;
		}

		public static bool ValidateHash(uint input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(ulong input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(Quaternion input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(Vector2 input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(Vector2Int input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(Vector3 input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(Vector3Int input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(float input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(double input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static bool ValidateHash(char[] input, int hash)
		{
			return CalculateHash(input) == hash;
		}
		
		public static int CalculateHashGeneric<T>(T input)
		{
			switch (input)
			{
				case BigInteger value:
					return CalculateHash(value);
				case bool value:
					return CalculateHash(value);	
				case byte value:
					return CalculateHash(value);	
				case char value:
					return CalculateHash(value);
				case DateTime value:
					return CalculateHash(value);
				case decimal value:
					return CalculateHash(value);	
				case double value:
					return CalculateHash(value);	
				case float value:
					return CalculateHash(value);	
				case int value:
					return CalculateHash(value);	
				case long value:
					return CalculateHash(value);	
				case Quaternion value:
					return CalculateHash(value);	
				case sbyte value:
					return CalculateHash(value);	
				case short value:
					return CalculateHash(value);	
				case string value:
					return CalculateHash(value.ToCharArray());	
				case char[] value:
					return CalculateHash(value);	
				case uint value:
					return CalculateHash(value);	
				case ulong value:
					return CalculateHash(value);	
				case ushort value:
					return CalculateHash(value);	
				case Vector2 value:
					return CalculateHash(value);	
				case Vector2Int value:
					return CalculateHash(value);	
				case Vector3 value:
					return CalculateHash(value);	
				case Vector3Int value:
					return CalculateHash(value);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static int CalculateHash(BigInteger x)
		{	
#if UNITY_2021_2_OR_NEWER
			var bytesCount = x.GetByteCount();
			var localBuffer = ArrayPool<byte>.Shared.Rent(bytesCount);
#else
			var localBuffer = x.ToByteArray(); 
            var bytesCount = localBuffer.Length;
#endif
			try
			{
#if UNITY_2021_2_OR_NEWER
				if (x.TryWriteBytes(localBuffer, out var bytesWritten))
					bytesCount = bytesWritten;
				else 
					throw new Exception("Unable to get bytes for hashing for " + x);
#endif
				
				var hash = xxHash.CalculateHash(localBuffer, bytesCount, Salt);
				unsafe
				{
					return *(int*)&hash;
				}
			}
			catch (Exception e)
			{
				Common.ACTk.PrintExceptionForSupport("Unable to calculate hash!" + x, e);
				throw;
			}
#if UNITY_2021_2_OR_NEWER
			finally
			{
				ArrayPool<byte>.Shared.Return(localBuffer);
			}
#endif
		}
		
		public static int CalculateHash(bool x)
		{
			return x ? 354242342 : 756756344 ^ Salt;
		}
		
		public static int CalculateHash(decimal x)
		{
			unsafe
			{
				var bits = (int*)&x;
				var high = ((long)bits[1] << 32) | (uint)bits[0];
				var low = ((long)bits[3] << 32) | (uint)bits[2];
				
				const int prime = 31;
				return unchecked(prime * CalculateHash(high) + CalculateHash(low) + Salt);
			}
		}
		
		public static int CalculateHash(int x) // doesn't return 0 for any input
		{
			const int a = 16777618;
			var hash = unchecked((int)2166136262);

			hash = (hash ^ (x & 0xff)) * a;
			hash = (hash ^ ((x >> 8) & 0xff)) * a;
			hash = (hash ^ ((x >> 16) & 0xff)) * a;
			hash = (hash ^ ((x >> 24) & 0xff)) * a;

			return hash | 1;
		}
		
		public static int CalculateHash(DateTime x)
		{
			return CalculateHash(x.ToBinary());
		}
		
		public static int CalculateHash(long x)
		{
			unsafe
			{
				var parts = (int*)&x;
				var low = parts[0];
				var high = parts[1];
        
				return (CalculateHash(low) >> 2) ^ CalculateHash(high);
			}
		}
		
		public static int CalculateHash(uint x)
		{
			int i;
			unsafe
			{
				i = *(int*)&x;
			}

			return CalculateHash(i);
		}
		
		public static int CalculateHash(ulong x)
		{
			unsafe
			{
				var parts = (int*)&x;
				var low = parts[0];
				var high = parts[1];
        
				return (CalculateHash(low) >> 2) ^ CalculateHash(high);
			}
		}
		
		public static int CalculateHash(float x)
		{
			int i;
			unsafe
			{
				i = *(int*)&x;
			}

			return CalculateHash(i);
		}
		
		public static int CalculateHash(double x)
		{
			long bits;
			unsafe
			{
				bits = *(long*)&x;
			}
			
			var hash = (int)(bits ^ (bits >> 32));
			const int prime = 31;
			return unchecked(prime * hash + (int)(bits >> 16) + Salt);
		}
		
		/*public static int CalculateHash(double x)
		{
			var bits = BitConverter.DoubleToInt64Bits(x);
			bits = (bits ^ (bits >> 33)) * unchecked((long)0xff51afd7ed558ccdL);
			bits = (bits ^ (bits >> 33)) * unchecked((long)0xc4ceb9fe1a85ec53L);
			return (int)(bits ^ (bits >> 32));
		}*/
		
		public static int CalculateHash(Quaternion input)
		{
			var xHash = CalculateHash(input.x);
			var yHash = CalculateHash(input.y) << 2;
			var zHash = CalculateHash(input.z) >> 2;
			var wHash = CalculateHash(input.w) >> 1;
			return (xHash ^ yHash ^ zHash ^ wHash) | 1;
		}
		
		public static int CalculateHash(Vector2 input)
		{
			var xHash = CalculateHash(input.x);
			var yHash = CalculateHash(input.y) << 2;
			return (xHash ^ yHash) | 1;
		}
		
		public static int CalculateHash(Vector2Int input)
		{
			var xHash = CalculateHash(input.x);
			var yHash = CalculateHash(input.y) << 2;
			return (xHash ^ yHash) | 1;
		}
		
		public static int CalculateHash(Vector3 input)
		{
			var xHash = CalculateHash(input.x);
			var yHash = CalculateHash(input.y) << 2;
			var zHash = CalculateHash(input.z) >> 2;
			return (xHash ^ yHash ^ zHash) | 1;
		}
		
		public static int CalculateHash(Vector3Int input)
		{
			var xHash = CalculateHash(input.x);
			var yHash = CalculateHash(input.y);
			var zHash = CalculateHash(input.z);
			return (xHash ^ yHash << 4 ^ yHash >> 28 ^ zHash >> 4 ^ zHash << 28) | 1;
		}
		
		public static int CalculateHash(char[] input)
		{
			var hash = Salt;
			
			if (input == null || input.Length == 0)
				return hash;

			unchecked
			{
				foreach (var c in input)
				{
					hash = ((hash << 5) + hash) ^ c;
				}
			}
			
			return hash | 1;
		}
	}
}