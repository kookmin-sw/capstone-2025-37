#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal class SerializedObscuredString : MigratableSerializedObscuredType<string>
	{
		public char[] Hidden
		{
			get => GetChars(HiddenProperty);
			set => SetChars(HiddenProperty, value);
		}

		public char[] Key
		{
			get => GetChars(KeyProperty);
			set => SetChars(KeyProperty, value);
		}
		
		public override bool IsDataValid => HashUtils.CalculateHash(Plain?.ToCharArray()) == Hash;

		public override string Plain => ObscuredString.Decrypt(Hidden, Key);
		public override bool IsCanMigrate => TryMigrateObsolete(Target, false, out _);
		protected override byte TypeVersion => ObscuredString.Version;
		
		protected override string HiddenPropertyRelativePath => nameof(ObscuredString.hiddenChars);
		protected override string KeyPropertyRelativePath => nameof(ObscuredString.cryptoKey);
		
		protected override bool PerformMigrate()
		{
			return TryMigrateObsolete(Target, true, out _);
		}
		
		private static char[] GetChars(SerializedProperty property)
		{
			var length = property.arraySize;
			var result = new char[length];
			for (var i = 0; i < length; i++)
				result[i] = (char)property.GetArrayElementAtIndex(i).intValue;

			return result;
		}
		
		public static void SetChars(SerializedProperty property, char[] array)
		{
			property.ClearArray();
			property.arraySize = array.Length;
			for (var i = 0; i < array.Length; i++)
				property.GetArrayElementAtIndex(i).intValue = array[i];
		}
		
		public static byte[] GetBytesObsolete(SerializedProperty property)
		{
			var length = property.arraySize;
			var result = new byte[length];
			for (var i = 0; i < length; i++)
				result[i] = (byte)property.GetArrayElementAtIndex(i).intValue;

			return result;
		}
		
		private bool TryMigrateObsolete(SerializedProperty sp, bool apply, out string value)
		{
			value = default;
			
			var hiddenValueProperty = sp.FindPropertyRelative(nameof(ObscuredString.hiddenValue));
			if (hiddenValueProperty == null) 
				return false;

			var currentCryptoKeyOldProperty = sp.FindPropertyRelative(nameof(ObscuredString.currentCryptoKey));
			if (currentCryptoKeyOldProperty == null) 
				return false;

			var currentCryptoKeyOld = currentCryptoKeyOldProperty.stringValue;
			if (string.IsNullOrEmpty(currentCryptoKeyOld)) 
				return false;

			var hiddenCharsProperty = sp.FindPropertyRelative(nameof(ObscuredString.hiddenChars));
			if (hiddenCharsProperty == null) 
				return false;

			var hiddenValue = GetBytesObsolete(hiddenValueProperty);
			var decrypted = ObscuredString.EncryptDecryptObsolete(ObscuredString.GetStringObsolete(hiddenValue), currentCryptoKeyOld);

			value = decrypted;
			
			if (apply)
			{
				var currentCryptoKey = ObscuredString.GenerateKey();
				var hiddenChars = ObscuredString.InternalEncryptDecrypt(decrypted.ToCharArray(), currentCryptoKey);

				SetChars(hiddenCharsProperty, hiddenChars);
				var currentCryptoKeyProperty = sp.FindPropertyRelative(nameof(ObscuredString.cryptoKey));
				SetChars(currentCryptoKeyProperty, currentCryptoKey);

				hiddenValueProperty.arraySize = 0;
				currentCryptoKeyOldProperty.stringValue = null;
			}

			return true;
		}
		
		public override string GetMigrationResultString()
		{
			return TryMigrateObsolete(Target, false, out var value) ? value : null;
		}
	}
}

#endif