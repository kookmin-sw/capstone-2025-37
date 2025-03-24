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
	/// Use it instead of regular <c>Vector2</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredVector2 : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal RawEncryptedVector2 hiddenValue;
		[SerializeField] internal int currentCryptoKey;

		internal Vector2 fakeValue;

		private ObscuredVector2(Vector2 value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = Encrypt(value, currentCryptoKey);
			hash = HashUtils.CalculateHash(value);
			fakeValue = ObscuredCheatingDetector.IsRunningInHoneypotMode ? value : default;

#if UNITY_EDITOR
			version = Version;
			migratedVersion = null;
#endif
		}

		/// <summary>
		/// Mimics constructor of regular Vector2.
		/// </summary>
		/// <param name="x">X component of the vector</param>
		/// <param name="y">Y component of the vector</param>
		public ObscuredVector2(float x, float y) : this(new Vector2(x, y)) { }

		public float x
		{
			get => InternalDecrypt().x;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredVector2(value, decrypted.y);
			}
		}

		public float y
		{
			get => InternalDecrypt().y;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredVector2(decrypted.x, value);
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					default:
						throw new IndexOutOfRangeException($"Invalid {nameof(ObscuredVector2)} index!");
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
					default:
						throw new IndexOutOfRangeException($"Invalid {nameof(ObscuredVector2)} index!");
				}
			}
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static RawEncryptedVector2 Encrypt(Vector2 value, int key)
		{
			return Encrypt(value.x, value.y, key);
		}

		/// <summary>
		/// Encrypts passed components using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static RawEncryptedVector2 Encrypt(float x, float y, int key)
		{
			RawEncryptedVector2 result;
			result.x = ObscuredFloat.Encrypt(x, key);
			result.y = ObscuredFloat.Encrypt(y, key);

			return result;
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static Vector2 Decrypt(RawEncryptedVector2 value, int key)
		{
			Vector2 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);

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
		public static ObscuredVector2 FromEncrypted(RawEncryptedVector2 encrypted, int key)
		{
			var instance = new ObscuredVector2();
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

		private static bool Compare(Vector2 v1, Vector2 v2)
		{
			return Compare(v1.x, v2.x) &&
			       Compare(v1.y, v2.y);
		}
		
		private static bool Compare(float f1, float f2)
		{
			var epsilon = ObscuredCheatingDetector.ExistsAndIsRunning ?
				ObscuredCheatingDetector.Instance.vector2Epsilon : float.Epsilon;
			return NumUtils.CompareWithEpsilon(f1, f2, epsilon);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public RawEncryptedVector2 GetEncrypted(out int key)
		{
			if (IsDefault()) this = new ObscuredVector2(default);

			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(RawEncryptedVector2 encrypted, int key)
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
		public Vector2 GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(Vector2 input, int hash)
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
		
		private void HideValue(Vector2 plain)
		{
			hiddenValue = Encrypt(plain, currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}

		private Vector2 InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredVector2(default);

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

		#endregion

		//! @endcond
	}
}