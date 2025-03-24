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
using UnityEngine.Serialization;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>float</c> for any cheating-sensitive properties, fields and other long-term declarations.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Use regular type for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredFloat : IObscuredType
	{
		[SerializeField] internal int hash;
		[SerializeField] internal int hiddenValue;
		[SerializeField] internal int currentCryptoKey;
		
		internal float fakeValue;

		private ObscuredFloat(float value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = Encrypt(value, currentCryptoKey);
			hash = HashUtils.CalculateHash(value);
			
			hiddenValueOldByte4 = default;

			fakeValue = ObscuredCheatingDetector.IsRunningInHoneypotMode ? value : default;

#if UNITY_EDITOR
			version = Version;
			migratedVersion = default;
#endif
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static int Encrypt(float value, int key)
		{
			return FloatIntBytesUnion.XorFloatToInt(value, key);
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static float Decrypt(int value, int key)
		{
			return FloatIntBytesUnion.XorIntToFloat(value, key);
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
		public static ObscuredFloat FromEncrypted(int encrypted, int key)
		{
			var instance = new ObscuredFloat();
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
		public int GetEncrypted(out int key)
		{
			if (IsDefault()) this = new ObscuredFloat(default);
			
			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(int encrypted, int key)
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
		public float GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			HideValue(decrypted);
		}
		
		private static bool ValidateHash(float input, int hash)
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
		
		private void HideValue(float plain)
		{
			hiddenValue = Encrypt(plain, currentCryptoKey);
			hash = HashUtils.CalculateHash(plain);
		}
		
		private static bool Compare(float f1, float f2)
		{
			var epsilon = ObscuredCheatingDetector.ExistsAndIsRunning ?
				ObscuredCheatingDetector.Instance.floatEpsilon : float.Epsilon;
			return NumUtils.CompareWithEpsilon(f1, f2, epsilon);
		}

		private float InternalDecrypt()
		{
			if (IsDefault()) this = new ObscuredFloat(default);
			
			var realValue = Decrypt(hiddenValue, currentCryptoKey);
			var hashValid = ValidateHash(realValue, hash);

			if (hashValid && fakeValue == default)
			{
				// init honeypot if it wasn't initialized yet
				if (realValue != default && ObscuredCheatingDetector.IsRunningInHoneypotMode)
					fakeValue = realValue;
			}

			var honeypotValid = Compare(realValue, fakeValue);
			ObscuredCheatingDetector.TryDetectCheating(this, hashValid, hash, honeypotValid, realValue, fakeValue); 
			
			return realValue;
		}

		private bool IsDefault()
		{
			return hiddenValue == default &&
				   currentCryptoKey == default &&
				   hash == default &&
				   fakeValue.Equals(default);
		}

		//! @cond

		#region obsolete
		
		[SerializeField] [FormerlySerializedAs("hiddenValue")]
#pragma warning disable 414
		internal ACTkByte4 hiddenValueOldByte4;
#pragma warning restore 414
		
		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public static void SetNewCryptoKey(int newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.", true)]
		public void ApplyNewCryptoKey() {}
		
		/// <summary>
		/// Allows to update the raw encrypted value to the newer encryption format.
		/// </summary>
		/// Use when you have some encrypted values saved somewhere with previous ACTk version
		/// and you wish to set them using SetEncrypted() to the newer ACTk version obscured type.
		/// Current migration variants:
		/// from 0 or 1 to 2 - migrate obscured type from ACTk 1.5.2.0-1.5.8.0 to the 1.5.9.0+ format
		/// <param name="encrypted">Encrypted value you got from previous ACTk version obscured type with GetEncrypted().</param>
		/// <param name="fromVersion">Source format version.</param>
		/// <param name="toVersion">Target format version.</param>
		/// <returns>Migrated raw encrypted value which you may use for SetEncrypted(0 later.</returns>
		[Obsolete("This will be removed in future versions.", false)]
		public static int MigrateEncrypted(int encrypted, byte fromVersion = 0, byte toVersion = 2)
		{
			return FloatIntBytesUnion.MigrateObsolete(encrypted, fromVersion, toVersion);
		}

		#endregion

		//! @endcond

		[StructLayout(LayoutKind.Explicit)]
		internal struct FloatIntBytesUnion
		{
			[FieldOffset(0)] private float f;
			[FieldOffset(0)] internal int i;
			[FieldOffset(0)] internal ACTkByte4 b4;

			public static int MigrateObsolete(int value, byte fromVersion, byte toVersion)
			{
				var u = FromInt(value);

				if (fromVersion < 2 && toVersion == 2)
					u.b4.Shuffle();

				return u.i;
			}

			internal static int XorFloatToInt(float value, int key)
			{
				return FromFloat(value).Shuffle(key).i;
			}

			internal static float XorIntToFloat(int value, int key)
			{
				return FromInt(value).UnShuffle(key).f;
			}

			private static FloatIntBytesUnion FromFloat(float value)
			{
				return new FloatIntBytesUnion { f = value};
			}

			private static FloatIntBytesUnion FromInt(int value)
			{
				return new FloatIntBytesUnion { i = value};
			}

			private FloatIntBytesUnion Shuffle(int key)
			{
				i ^= key;
				b4.Shuffle();

				return this;
			}

			private FloatIntBytesUnion UnShuffle(int key)
			{
				b4.UnShuffle();
				i ^= key;

				return this;
			}
		}
	}
}