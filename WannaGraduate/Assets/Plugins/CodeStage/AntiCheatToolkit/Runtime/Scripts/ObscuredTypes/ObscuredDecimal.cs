#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Utils;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>decimal</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredDecimal : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal ACTkByte16 hiddenValue;
		[SerializeField] internal long currentCryptoKey;

		private decimal fakeValue;

		private ObscuredDecimal(decimal value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = InternalEncrypt(value, currentCryptoKey);
			hash = HashUtils.CalculateHash(value);
			
			fakeValue = ObscuredCheatingDetector.IsRunningInHoneypotMode ? value : default;

#if UNITY_EDITOR
			version = Version;
#endif
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static decimal Encrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToDecimal(value, key);
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static decimal Decrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToDecimal(value, key);
		}

		/// <summary>
		/// Creates and fills obscured variable with raw encrypted value previously got from GetEncrypted().
		/// </summary>
		/// Literally does same job as SetEncrypted() but makes new instance instead of filling existing one,
		/// making it easier to initialize new variables from saved encrypted values.
		///
		/// <param name="encrypted">Raw encrypted value you got from GetEncrypted().</param>
		/// <param name="key">Encryption key you've got from GetEncrypted().</param>
		/// <returns>New obscured variable initialized from specified encrypted value.</returns>
		/// \sa GetEncrypted(), SetEncrypted()
		public static ObscuredDecimal FromEncrypted(decimal encrypted, long key)
		{
			var instance = new ObscuredDecimal();
			instance.SetEncrypted(encrypted, key);
			return instance;
		}

		/// <summary>
		/// Generates random key. Used internally and can be used to generate key for manual Encrypt() calls.
		/// </summary>
		/// <returns>Key suitable for manual Encrypt() calls.</returns>
		public static long GenerateKey()
		{
			return RandomUtils.GenerateLongKey();
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public decimal GetEncrypted(out long key)
		{
			if (IsDefault()) this = new ObscuredDecimal(default);
			
			key = currentCryptoKey;
			return DecimalLongBytesUnion.ConvertToDecimal(hiddenValue);
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(decimal encrypted, long key)
		{
			currentCryptoKey = key;
			var plain = Decrypt(encrypted, key);
			hiddenValue = DecimalLongBytesUnion.ConvertToB16(encrypted);
			hash = HashUtils.CalculateHash(plain);

			if (ObscuredCheatingDetector.IsRunningInHoneypotMode)
				fakeValue = plain;
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public decimal GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(decimal input, int hash)
		{
#if DEBUG
			if (hash == default && HashUtils.CalculateHash(input) != default)
				Debug.LogError(ACTk.LogPrefix + $"{nameof(hash)} is not initialized properly!\n" +
							   "It will produce false positive cheating detection.\n" +
							   "Can happen when migrating from older ACTk versions.\n" +
							   "Please call Tools > Code Stage > Anti-Cheat Toolkit > Migrate > * menu item to try fixing this.");
#endif
			return HashUtils.ValidateHash(input, hash);
		}

		private void HideValue(decimal plain)
		{
			hiddenValue = InternalEncrypt(plain, currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}

		private static ACTkByte16 InternalEncrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToB16(value, key);
		}

		private decimal InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredDecimal(default);

			var plain = DecimalLongBytesUnion.XorB16ToDecimal(hiddenValue, currentCryptoKey);
			var hashValid = ValidateHash(plain, hash);
			
			if (hashValid && fakeValue == default)
			{
				// init honeypot if it wasn't initialized yet
				if (plain != default && ObscuredCheatingDetector.IsRunningInHoneypotMode)
					fakeValue = plain;
			}			

			var honeypotValid = plain == fakeValue;
			ObscuredCheatingDetector.TryDetectCheating(this, hashValid, hash, honeypotValid, plain, fakeValue); 
			
			return plain;
		}
		
		private bool IsDefault()
		{
			return hiddenValue.Equals(default) &&
				   currentCryptoKey == default &&
				   hash == default &&
				   fakeValue.Equals(default);
		}

		//! @cond

		#region obsolete

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public static void SetNewCryptoKey(long newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public void ApplyNewCryptoKey() {}

		#endregion

		//! @endcond
	}
	
	[StructLayout(LayoutKind.Explicit)]
	internal struct DecimalLongBytesUnion
	{
		[FieldOffset(0)]
		internal decimal d;

		[FieldOffset(0)]
		internal long l1;

		[FieldOffset(8)]
		internal long l2;

		[FieldOffset(0)]
		internal ACTkByte16 b16;

		internal static decimal XorDecimalToDecimal(decimal value, long key)
		{
			return FromDecimal(value).XorLongs(key).d;
		}

		internal static ACTkByte16 XorDecimalToB16(decimal value, long key)
		{
			return FromDecimal(value).XorLongs(key).b16;
		}

		internal static decimal XorB16ToDecimal(ACTkByte16 value, long key)
		{
			return FromB16(value).XorLongs(key).d;
		}

		internal static decimal ConvertToDecimal(ACTkByte16 value)
		{
			return FromB16(value).d;
		}

		internal static ACTkByte16 ConvertToB16(decimal value)
		{
			return FromDecimal(value).b16;
		}

		internal static DecimalLongBytesUnion FromDecimal(decimal value)
		{
			return new DecimalLongBytesUnion {d = value};
		}

		private static DecimalLongBytesUnion FromB16(ACTkByte16 value)
		{
			return new DecimalLongBytesUnion {b16 = value};
		}

		private DecimalLongBytesUnion XorLongs(long key)
		{
			l1 ^= key;
			l2 ^= key;
			return this;
		}
	}
}