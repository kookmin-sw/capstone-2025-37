#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Utils;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Vector3Int</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredVector3Int : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal RawEncryptedVector3Int hiddenValue;
		[SerializeField] internal int currentCryptoKey;
		
		internal Vector3Int fakeValue;

		private ObscuredVector3Int(Vector3Int value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = Encrypt(value, currentCryptoKey);
			hash = HashUtils.CalculateHash(value);
			fakeValue = ObscuredCheatingDetector.IsRunningInHoneypotMode ? value : default;

#if UNITY_EDITOR
			version = Version;
#endif
		}

		/// <summary>
		/// Mimics constructor of regular Vector3Int.
		/// </summary>
		/// <param name="x">X component of the vector</param>
		/// <param name="y">Y component of the vector</param>
		/// <param name="z">Z component of the vector</param>
		public ObscuredVector3Int(int x, int y, int z) : this(new Vector3Int(x, y, z)) { }

		public int x
		{
			get => InternalDecrypt().x;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredVector3Int(value, decrypted.y, decrypted.z);
			}
		}

		public int y
		{
			get => InternalDecrypt().y;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredVector3Int(decrypted.x, value, decrypted.z);
			}
		}

		public int z
		{
			get => InternalDecrypt().z;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredVector3Int(decrypted.x, decrypted.y, value);
			}
		}

		public int this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException($"Invalid {nameof(ObscuredVector3Int)} index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException($"Invalid {nameof(ObscuredVector3Int)} index!");
				}
			}
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static RawEncryptedVector3Int Encrypt(Vector3Int value, int key)
		{
			return Encrypt(value.x, value.y, value.z, key);
		}

		/// <summary>
		/// Encrypts passed components using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static RawEncryptedVector3Int Encrypt(int x, int y, int z, int key)
		{
			RawEncryptedVector3Int result;
			result.x = ObscuredInt.Encrypt(x, key);
			result.y = ObscuredInt.Encrypt(y, key);
			result.z = ObscuredInt.Encrypt(z, key);

			return result;
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static Vector3Int Decrypt(RawEncryptedVector3Int value, int key)
		{
			var result = new Vector3Int
			{
				x = ObscuredInt.Decrypt(value.x, key),
				y = ObscuredInt.Decrypt(value.y, key),
				z = ObscuredInt.Decrypt(value.z, key)
			};

			return result;
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
		public static ObscuredVector3Int FromEncrypted(RawEncryptedVector3Int encrypted, int key)
		{
			var instance = new ObscuredVector3Int();
			instance.SetEncrypted(encrypted, key);
			return instance;
		}

		/// <summary>
		/// Generates random key. Used internally and can be used to generate key for manual Encrypt() calls.
		/// </summary>
		/// <returns>Key suitable for manual Encrypt() calls.</returns>
		public static int GenerateKey()
		{
			return RandomUtils.GenerateIntKey();
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public RawEncryptedVector3Int GetEncrypted(out int key)
		{
			if (IsDefault()) this = new ObscuredVector3Int(default);

			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(RawEncryptedVector3Int encrypted, int key)
		{
			currentCryptoKey = key;
			var plain = Decrypt(encrypted, key);
			hiddenValue = encrypted;
			hash = HashUtils.CalculateHash(plain);

			if (ObscuredCheatingDetector.IsRunningInHoneypotMode)
				fakeValue = plain;
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public Vector3Int GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(Vector3Int input, int hash)
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
		
		private void HideValue(Vector3Int plain)
		{
			hiddenValue = Encrypt(plain, currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}

		private Vector3Int InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredVector3Int(default);

			var realValue = Decrypt(hiddenValue, currentCryptoKey);
			var hashValid = ValidateHash(realValue, hash);

			if (hashValid && fakeValue == default)
			{
				// init honeypot if it wasn't initialized yet
				if (realValue != default && ObscuredCheatingDetector.IsRunningInHoneypotMode)
					fakeValue = realValue;
			}

			var honeypotValid = realValue == fakeValue;
			ObscuredCheatingDetector.TryDetectCheating(this, hashValid, hash, honeypotValid, realValue, fakeValue); 
			
			return realValue;
		}
		
		private bool IsDefault()
		{
			return hiddenValue.x == default &&
				   hiddenValue.y == default &&
				   hiddenValue.z == default &&
				   currentCryptoKey == default &&
				   hash == default &&
				   fakeValue.Equals(default);
		}

		//! @cond

		#region obsolete

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public static void SetNewCryptoKey(int newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public void ApplyNewCryptoKey() {}

		/// <summary>
		/// Decrypts data encrypted in ACTk 2024.0 or earlier.
		/// </summary>
		public static Vector3Int DecryptFromV0(RawEncryptedVector3Int value, int key)
		{
			var result = new Vector3Int
			{
				x = ObscuredInt.DecryptFromV0(value.x, key), 
				y = ObscuredInt.DecryptFromV0(value.y, key),
				z = ObscuredInt.DecryptFromV0(value.z, key)
			};

			return result;
		}

		#endregion

		//! @endcond
	}
}