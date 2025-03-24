#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

// ACTK_NEWTONSOFT_JSON is True if project has the com.unity.nuget.newtonsoft-json package v2.0.0 or newer installed
// you can add this package to your project via the Package Manager by package name and use it instead of regular Json.NET library
// or you can enable ACTK_NEWTONSOFT_JSON manually at ACTk Settings instead.

#if ACTK_NEWTONSOFT_JSON

using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Globalization;

namespace CodeStage.AntiCheat.ObscuredTypes.Converters
{
	/// <summary>
	/// Regular JsonConverter for Jscon.NET that allows to serialize and deserialize ObscuredTypes decrypted values.
	/// </summary>
	/// See more and usage examples at the 'Obscured Types JSON Serialization' User Manual chapter.
	public class ObscuredTypesNewtonsoftConverter : JsonConverter 
	{
		private static readonly string X = string.Intern("x");
		private static readonly string Y = string.Intern("y");
		private static readonly string Z = string.Intern("z");
		private static readonly string W = string.Intern("w");
		
        private readonly HashSet<Type> types = new HashSet<Type> {
            typeof(ObscuredBigInteger),
			typeof(ObscuredBool),
			typeof(ObscuredByte),
			typeof(ObscuredChar),
			typeof(ObscuredDateTime),
			typeof(ObscuredDecimal),
			typeof(ObscuredDouble),
            typeof(ObscuredFloat),
			typeof(ObscuredInt),
            typeof(ObscuredLong),
            typeof(ObscuredQuaternion),
            typeof(ObscuredSByte),
            typeof(ObscuredShort),
			typeof(ObscuredString),
            typeof(ObscuredUInt),
            typeof(ObscuredULong),
            typeof(ObscuredUShort),
            typeof(ObscuredVector2),
            typeof(ObscuredVector2Int),
            typeof(ObscuredVector3),
            typeof(ObscuredVector3Int),
        };

		private readonly Dictionary<Type, Action<JsonWriter, object>> writeActions =
			new Dictionary<Type, Action<JsonWriter, object>>
			{
				{ typeof(ObscuredBigInteger), (writer, value) => writer.WriteValue(((ObscuredBigInteger)value).ToString(CultureInfo.InvariantCulture)) },
				{ typeof(ObscuredBool), (writer, value) => writer.WriteValue((ObscuredBool)value) },
				{ typeof(ObscuredByte), (writer, value) => writer.WriteValue((ObscuredByte)value) },
				{ typeof(ObscuredChar), (writer, value) => writer.WriteValue((ObscuredChar)value) },
				{ typeof(ObscuredDateTime), (writer, value) => writer.WriteValue((ObscuredDateTime)value) },
				{ typeof(ObscuredDecimal), (writer, value) => writer.WriteValue((ObscuredDecimal)value) },
				{ typeof(ObscuredDouble), (writer, value) => writer.WriteValue((ObscuredDouble)value) },
				{ typeof(ObscuredFloat), (writer, value) => writer.WriteValue((ObscuredFloat)value) },
				{ typeof(ObscuredInt), (writer, value) => writer.WriteValue((ObscuredInt)value) },
				{ typeof(ObscuredLong), (writer, value) => writer.WriteValue((ObscuredLong)value) },
				{ typeof(ObscuredQuaternion), (writer, value) => WriteQuaternion(writer, (ObscuredQuaternion)value) },
				{ typeof(ObscuredSByte), (writer, value) => writer.WriteValue((ObscuredSByte)value) },
				{ typeof(ObscuredShort), (writer, value) => writer.WriteValue((ObscuredShort)value) },
				{ typeof(ObscuredString), (writer, value) => writer.WriteValue((ObscuredString)value) },
				{ typeof(ObscuredUInt), (writer, value) => writer.WriteValue((ObscuredUInt)value) },
				{ typeof(ObscuredULong), (writer, value) => writer.WriteValue((ObscuredULong)value) },
				{ typeof(ObscuredUShort), (writer, value) => writer.WriteValue((ObscuredUShort)value) },
				{ typeof(ObscuredVector2), (writer, value) => WriteVector2(writer, (ObscuredVector2)value) },
				{ typeof(ObscuredVector2Int), (writer, value) => WriteVector2Int(writer, (ObscuredVector2Int)value) },
				{ typeof(ObscuredVector3), (writer, value) => WriteVector3(writer, (ObscuredVector3)value) },
				{ typeof(ObscuredVector3Int), (writer, value) => WriteVector3Int(writer, (ObscuredVector3Int)value) },
			};
 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var type = value?.GetType();
			if (type != null && writeActions.TryGetValue(type, out var action))
			{
				action(writer, value);
			}
			else
			{
				throw new NotSupportedException($"{nameof(ObscuredTypesNewtonsoftConverter)} type " + value?.GetType() + " is not implemented!");
			}
		}

		private static void WriteQuaternion(JsonWriter writer, Quaternion quaternion)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(X);
			writer.WriteValue(quaternion.x);
			writer.WritePropertyName(Y);
			writer.WriteValue(quaternion.y);
			writer.WritePropertyName(Z);
			writer.WriteValue(quaternion.z);
			writer.WritePropertyName(W);
			writer.WriteValue(quaternion.w);
			writer.WriteEndObject();
		}
		
		private static void WriteVector2(JsonWriter writer, Vector2 vector2)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(X);
			writer.WriteValue(vector2.x);
			writer.WritePropertyName(Y);
			writer.WriteValue(vector2.y);
			writer.WriteEndObject();
		}
		
		private static void WriteVector2Int(JsonWriter writer, Vector2Int vector2Int)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(X);
			writer.WriteValue(vector2Int.x);
			writer.WritePropertyName(Y);
			writer.WriteValue(vector2Int.y);
			writer.WriteEndObject();
		}
		
		private static void WriteVector3(JsonWriter writer, Vector3 vector3)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(X);
			writer.WriteValue(vector3.x);
			writer.WritePropertyName(Y);
			writer.WriteValue(vector3.y);
			writer.WritePropertyName(Z);
			writer.WriteValue(vector3.z);
			writer.WriteEndObject();
		}
		
		private static void WriteVector3Int(JsonWriter writer, Vector3Int vector3Int)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(X);
			writer.WriteValue(vector3Int.x);
			writer.WritePropertyName(Y);
			writer.WriteValue(vector3Int.y);
			writer.WritePropertyName(Z);
			writer.WriteValue(vector3Int.z);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
		    if (reader.TokenType == JsonToken.Null)
		        return null;
			
			if (objectType == typeof(ObscuredBigInteger))
			{
				var input = reader.Value as string;
				if (string.IsNullOrEmpty(input))
					return (ObscuredBigInteger)0;
				
				if (BigInteger.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var output))
					return (ObscuredBigInteger)output;
				
				// Obsolete fallback to read bytearray
				var bytes = Convert.FromBase64String(input);
				return (ObscuredBigInteger)new BigInteger(bytes);
			}
		    if (objectType == typeof(ObscuredBool))
		        return (ObscuredBool)Convert.ToBoolean(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredByte))
		        return (ObscuredByte)Convert.ToByte(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredChar))
		        return (ObscuredChar)Convert.ToChar(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredDateTime))
		        return (ObscuredDateTime)Convert.ToDateTime(reader.Value, reader.Culture);
			if (objectType == typeof(ObscuredDecimal))
		        return (ObscuredDecimal)Convert.ToDecimal(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredDouble))
		        return (ObscuredDouble)Convert.ToDouble(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredFloat))
		        return (ObscuredFloat)Convert.ToSingle(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredInt))
		        return (ObscuredInt)Convert.ToInt32(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredLong))
		        return (ObscuredLong)Convert.ToInt64(reader.Value, reader.Culture);

		    if (objectType == typeof(ObscuredQuaternion))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredQuaternion)}.");
				
		        var jsonObject = JObject.Load(reader);
				if (jsonObject == null)
					throw new Exception($"Couldn't load {nameof(JObject)} for {nameof(ObscuredQuaternion)}.");
				
		        var x = (jsonObject[X] ?? 0).Value<float>();
		        var y = (jsonObject[Y] ?? 0).Value<float>();
		        var z = (jsonObject[Z] ?? 0).Value<float>();
		        var w = (jsonObject[W] ?? 0).Value<float>();
		        var quaternion = new Quaternion(x, y, z, w);
		        return (ObscuredQuaternion)quaternion;
		    }

		    if (objectType == typeof(ObscuredSByte))
		        return (ObscuredSByte)Convert.ToSByte(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredShort))
		        return (ObscuredShort)Convert.ToInt16(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredString))
		        return (ObscuredString)Convert.ToString(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredUInt))
		        return (ObscuredUInt)Convert.ToUInt32(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredULong))
		        return (ObscuredULong)Convert.ToUInt64(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredUShort))
		        return (ObscuredUShort)Convert.ToUInt16(reader.Value, reader.Culture);

		    if (objectType == typeof(ObscuredVector2))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector2)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject[X] ?? 0).Value<float>();
		        var y = (jsonObject[Y] ?? 0).Value<float>();
		        var vector2 = new Vector2(x, y);
		        return (ObscuredVector2)vector2;
		    }

		    if (objectType == typeof(ObscuredVector2Int))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector2Int)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject[X] ?? 0).Value<int>();
		        var y = (jsonObject[Y] ?? 0).Value<int>();
		        var vector2Int = new Vector2Int(x, y);
		        return (ObscuredVector2Int)vector2Int;
		    }

		    if (objectType == typeof(ObscuredVector3))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector3)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject[X] ?? 0).Value<float>();
		        var y = (jsonObject[Y] ?? 0).Value<float>();
		        var z = (jsonObject[Z] ?? 0).Value<float>();
				var vector3 = new Vector3(x, y, z);
		        return (ObscuredVector3)vector3;
		    }

		    if (objectType == typeof(ObscuredVector3Int))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector3)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject[X] ?? 0).Value<int>();
		        var y = (jsonObject[Y] ?? 0).Value<int>();
		        var z = (jsonObject[Z] ?? 0).Value<int>();
				var vector3Int = new Vector3Int(x, y, z);
		        return (ObscuredVector3Int)vector3Int;
		    }

		    throw new NotSupportedException($"{nameof(ObscuredTypesNewtonsoftConverter)} type {objectType} is not implemented!");
		}
		
        public override bool CanConvert(Type objectType) {
            return types.Contains(objectType);
        }
	}
}

#endif