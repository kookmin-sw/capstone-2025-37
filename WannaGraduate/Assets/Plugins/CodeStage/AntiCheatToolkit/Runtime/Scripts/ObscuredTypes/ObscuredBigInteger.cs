#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Numerics;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Utils;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>BigInteger</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredBigInteger : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal SerializableBigInteger hiddenValue;
		[SerializeField] internal uint currentCryptoKey;
		
		internal BigInteger fakeValue;

		private ObscuredBigInteger(BigInteger value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = new SerializableBigInteger
			{
				value = new BigInteger(value.ToByteArray())
			};
			hiddenValue = hiddenValue.Encrypt(currentCryptoKey);
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
		public static BigInteger Encrypt(BigInteger value, uint key)
		{
			var input = new SerializableBigInteger { value = new BigInteger(value.ToByteArray()) };
			input = input.Encrypt(key);
			return input;
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static BigInteger Decrypt(BigInteger value, uint key)
		{
			var input = new SerializableBigInteger { value = value };
			return input.Decrypt(key);
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
		public static ObscuredBigInteger FromEncrypted(BigInteger encrypted, uint key)
		{
			var instance = new ObscuredBigInteger();
			instance.SetEncrypted(encrypted, key);
			return instance;
		}

		/// <summary>
		/// Generates random key. Used internally and can be used to generate key for manual Encrypt() calls.
		/// </summary>
		/// <returns>Key suitable for manual Encrypt() calls.</returns>
		public static uint GenerateKey()
		{
			return RandomUtils.GenerateUIntKey();
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public BigInteger GetEncrypted(out uint key)
		{
			if (IsDefault()) this = new ObscuredBigInteger(default);
			
			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(BigInteger encrypted, uint key)
		{
			currentCryptoKey = key;
			var plain = Decrypt(encrypted, key);
			hiddenValue.value = encrypted;
			hash = HashUtils.CalculateHash(plain);

			if (ObscuredCheatingDetector.IsRunningInHoneypotMode)
				fakeValue = plain;
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public BigInteger GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(BigInteger input, int hash)
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
		
		private void HideValue(BigInteger plain)
		{
			hiddenValue.value = plain;
			hiddenValue = hiddenValue.Encrypt(currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}

		private BigInteger InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredBigInteger(default);

			var plain = hiddenValue.Decrypt(currentCryptoKey);
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
			return hiddenValue.raw.Equals(default) &&
				   currentCryptoKey == default &&
				   hash == default &&
				   fakeValue.Equals(default);
		}
	}
}
