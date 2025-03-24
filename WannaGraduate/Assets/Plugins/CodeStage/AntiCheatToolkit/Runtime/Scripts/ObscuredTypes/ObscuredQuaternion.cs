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
	/// Use it instead of regular <c>Quaternion</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredQuaternion : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal RawEncryptedQuaternion hiddenValue;
		[SerializeField] internal int currentCryptoKey;
		
		internal Quaternion fakeValue;

		private ObscuredQuaternion(Quaternion value)
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
		/// Mimics constructor of regular Quaternion. Please note, passed components are not Euler Angles.
		/// </summary>
		/// <param name="x">X component of the quaternion</param>
		/// <param name="y">Y component of the quaternion</param>
		/// <param name="z">Z component of the quaternion</param>
		/// <param name="w">W component of the quaternion</param>
		public ObscuredQuaternion(float x, float y, float z, float w) : this(new Quaternion(x, y, z, w)) { }

		public float x
		{
			get => InternalDecrypt().x;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredQuaternion(value, decrypted.y, decrypted.z, decrypted.w);
			}
		}

		public float y
		{
			get => InternalDecrypt().y;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredQuaternion(decrypted.x, value, decrypted.z, decrypted.w);
			}
		}

		public float z
		{
			get => InternalDecrypt().z;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredQuaternion(decrypted.x, decrypted.y, value, decrypted.w);
			}
		}
		
		public float w
		{
			get => InternalDecrypt().w;
			set
			{
				var decrypted = InternalDecrypt();
				this = new ObscuredQuaternion(decrypted.x, decrypted.y, decrypted.z, value);
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
					case 2:
						return z;
					case 3:
						return w;
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
					case 3:
						w = value;
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
		public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			return Encrypt(value.x, value.y, value.z, value.w, key);
		}

		/// <summary>
		/// Encrypts passed components using passed key.
		/// Please note, passed components are not an Euler Angles.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static RawEncryptedQuaternion Encrypt(float x, float y, float z, float w, int key)
		{
			RawEncryptedQuaternion result;
			result.x = ObscuredFloat.Encrypt(x, key);
			result.y = ObscuredFloat.Encrypt(y, key);
			result.z = ObscuredFloat.Encrypt(z, key);
			result.w = ObscuredFloat.Encrypt(w, key);

			return result;
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
		{
			Quaternion result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);

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
		public static ObscuredQuaternion FromEncrypted(RawEncryptedQuaternion encrypted, int key)
		{
			var instance = new ObscuredQuaternion();
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

		private static bool Compare(Quaternion q1, Quaternion q2)
		{
			var epsilon = ObscuredCheatingDetector.ExistsAndIsRunning ?
				ObscuredCheatingDetector.Instance.quaternionEpsilon : float.Epsilon;
			return NumUtils.CompareWithEpsilon(q1.x, q2.x, epsilon) &&
			       NumUtils.CompareWithEpsilon(q1.y, q2.y, epsilon) &&
			       NumUtils.CompareWithEpsilon(q1.z, q2.z, epsilon) &&
			       NumUtils.CompareWithEpsilon(q1.w, q2.w, epsilon);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public RawEncryptedQuaternion GetEncrypted(out int key)
		{
			if (IsDefault()) this = new ObscuredQuaternion(default);

			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(RawEncryptedQuaternion encrypted, int key)
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
		public Quaternion GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(Quaternion input, int hash)
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
		
		private void HideValue(Quaternion plain)
		{
			hiddenValue = Encrypt(plain, currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}

		private Quaternion InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredQuaternion(default);

			var realValue = Decrypt(hiddenValue, currentCryptoKey);
			var hashValid = ValidateHash(realValue, hash);

			if (hashValid && fakeValue == default)
			{
				// init honeypot if it wasn't initialized yet
				if (realValue != default && ObscuredCheatingDetector.IsRunningInHoneypotMode)
					fakeValue = realValue;
			}

			var honeypotValid = realValue.Equals(fakeValue);
			ObscuredCheatingDetector.TryDetectCheating(this, hashValid, hash, honeypotValid, realValue, fakeValue); 
			
			return realValue;
		}
		
		private bool IsDefault()
		{
			return hiddenValue.x == default &&
				   hiddenValue.y == default &&
				   hiddenValue.z == default &&
				   hiddenValue.w == default &&
				   currentCryptoKey == default &&
				   hash == default &&
				   fakeValue.Equals(default);
		}

		#region obsolete
		
		//! @cond

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public static void SetNewCryptoKey(int newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public void ApplyNewCryptoKey() {}
		
		//! @endcond

		#endregion
	}
}