#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Storage
{
	using System.Collections.Generic;

	internal static class PrefsSerialization
	{
		public static ObscuredPrefsData SerializeStorageDataType<T>(T value, SerializationSettings settings)
		{
			var serializer = GetSerializer(settings);
			return serializer.SerializeStorageDataType(value);
		}

		public static T DeserializeStorageDataType<T>(ObscuredPrefsData data, SerializationSettings settings)
		{
			var serializer = GetSerializer(settings);
			return serializer.DeserializeStorageDataType<T>(data);
		}

		public static byte[] SerializePrefsDictionary(Dictionary<string, ObscuredPrefsData> value, SerializationSettings settings)
		{
			var serializer = GetSerializer(settings);
			return serializer.SerializePrefsDictionary(value);
		}
		
		public static Dictionary<string, ObscuredPrefsData> DeserializePrefsDictionary(byte[] data, SerializationSettings settings)
		{
			var serializer = GetSerializer(settings);
			return serializer.DeserializePrefsDictionary(data);
		}
		
		private static IObscuredFilePrefsSerializer GetSerializer(SerializationSettings settings)
		{
			return settings.SerializationKind == ACTkSerializationKind.Binary ? 
				BinarySerializer.GetSerializer() : JsonSerializer.GetSerializer();
		}
	}
}