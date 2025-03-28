﻿#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

// add this line in order to use any obscured types from code
using CodeStage.AntiCheat.ObscuredTypes;

using System;
using System.Numerics;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CodeStage.AntiCheat.Examples
{
	internal partial class ObscuredTypesExamples : MonoBehaviour
	{
		[Header("Settings")]
		public bool randomize = true;
		
		[Header("Regular variables")]
		public string regularString = "I'm regular string";

		public int regularInt = 1987;
		public float regularFloat = 2013.0524f;
		public Vector3 regularVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		// you can declare obscured variables like this and they will
		// appear in the inspector
		//
		// you can change values in inspector and they will be
		// automatically encrypted under the hood
		[Header("Obscured (secure) variables")]
		public ObscuredString obscuredString = "I'm obscured string";
		public ObscuredInt obscuredInt = 1987;
		public ObscuredFloat obscuredFloat = 2013.0524f;
		public ObscuredVector3 obscuredVector3 = new Vector3(10.5f, 11.5f, 12.5f);
		public ObscuredBool obscuredBool = true;
		public ObscuredLong obscuredLong = 945678987654123345L;
		public ObscuredDouble obscuredDouble = 9.45678987654d;
		public ObscuredVector2 obscuredVector2 = new Vector2(8.5f, 9.5f);
		public ObscuredDecimal obscuredDecimal = 503.4521m;
		public ObscuredVector2Int obscuredVector2Int = new Vector2Int(8, 9);
		public ObscuredVector3Int obscuredVector3Int = new Vector3Int(15, 16, 17);
		public ObscuredBigInteger obscuredBigInteger = BigInteger.Multiply(293847239847, 12312313);
		public ObscuredDateTime obscuredDateTime = new DateTime(2023, 05, 19, 13, 57, 00, DateTimeKind.Utc);

		private readonly StringBuilder logBuilder = new StringBuilder();

		private void Awake()
		{
			ObscuredStringExample();
			ObscuredIntExample();
		}

		private void Start()
		{
			// values set in inspector can be used as usual

			logBuilder.Length = 0;
			logBuilder.AppendLine("obscuredString value from inspector: " + obscuredString);
			logBuilder.AppendLine("ObscuredDecimal value from inspector: " + obscuredDecimal);
			logBuilder.AppendLine("ObscuredBool value from inspector: " + obscuredBool);
			logBuilder.AppendLine("ObscuredLong value from inspector: " + obscuredLong);
			logBuilder.AppendLine("ObscuredDouble value from inspector: " + obscuredDouble);
			logBuilder.AppendLine("ObscuredVector2 value from inspector: " + obscuredVector2);
			logBuilder.AppendLine("ObscuredVector2Int value from inspector: " + obscuredVector2Int);
			logBuilder.AppendLine("ObscuredVector3Int value from inspector: " + obscuredVector3Int);
			logBuilder.AppendLine("ObscuredBigInteger value from inspector: " + obscuredBigInteger);
			logBuilder.AppendLine("ObscuredDateTime value from inspector: " + obscuredDateTime);

			Debug.Log(logBuilder);

			StartRandomization();
		}

		private void StartRandomization()
		{
			// to make it harder to find encrypted value with unknown value search,
			// you can change encrypted value at the unexpected moments so cheater
			// can't guess if it was changed or not
			//
			// check RandomizeCryptoKey() API docs for more details
			Invoke(nameof(RandomizeObscuredVars), 1f);
			Debug.Log("Crypto keys randomization started");
		}

		private void StopRandomization()
		{
			CancelInvoke(nameof(RandomizeObscuredVars));
			Debug.Log("Crypto keys randomization cancelled");
		}

		private void RandomizeObscuredVars()
		{
			obscuredInt.RandomizeCryptoKey();
			obscuredFloat.RandomizeCryptoKey();
			obscuredString.RandomizeCryptoKey();
			obscuredVector3.RandomizeCryptoKey();

			// change interval each time to make it inconsistent
			Invoke(nameof(RandomizeObscuredVars), Random.Range(1f, 30f));
		}

		private void ObscuredStringExample()
		{
			/* -------------- usage example -------------- */

			// hey, Daniele! ;D
			var regular = "the Goscurry is not a lie ;)";

			// obscured <-> regular conversion is implicit
			ObscuredString obscured = regular;

			// you can get raw encrypted value at any time
			// and save it somewhere for example along with encryptionKey
			char[] encryptionKey;
			var encryptedValueRaw = obscured.GetEncrypted(out encryptionKey);

			// to construct new obscured instance from it after loading it back
			var newObscured = ObscuredString.FromEncrypted(encryptedValueRaw, encryptionKey);

			// all other obscured types have similar usage pipeline and APIs

			/* -------------- logs-------------- */

			logBuilder.Length = 0;
			logBuilder.AppendLine("[ ObscuredString example ]");
			logBuilder.AppendLine("Original value:\n" + regular);
			logBuilder.AppendLine("Obscured value in memory:\n" + new string(newObscured.GetEncrypted(out encryptionKey)));
			Debug.Log(logBuilder);
		}

		private void ObscuredIntExample()
		{
			/* -------------- usage example -------------- */

			var regular = 5;

			// obscured <-> regular conversion is implicit
			var obscured = (ObscuredInt)regular;

			// all usual operations are supported
			regular = obscured;
			obscured = regular;
			obscured -= 2;
			obscured = obscured + regular + 10;
			obscured = obscured / 2;
			obscured++;
			obscured--;

			// all other obscured types have similar usage pipeline and APIs

			/* -------------- logs-------------- */

			logBuilder.Length = 0;
			logBuilder.AppendLine("[ ObscuredInt example ]");
			logBuilder.AppendLine("Original lives count: " + regular);
			int key;
			logBuilder.AppendLine("Obscured lives count in memory: " + ((ObscuredInt)regular).GetEncrypted(out key));
			logBuilder.AppendLine("Lives count after few operations with obscured value: " + obscured + " (" + obscured.ToString("X") + "h)");

			Debug.Log(logBuilder);
		}
	}
}