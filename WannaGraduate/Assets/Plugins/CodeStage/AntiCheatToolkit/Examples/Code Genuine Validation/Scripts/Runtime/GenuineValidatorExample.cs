#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples.Genuine
{
	using Utils;
	using UnityEngine;
	using CodeStage.AntiCheat.Genuine.CodeHash;
	using System;
	using System.IO;
	using ObscuredTypes;

	// use this to check hash generated with CodeHashGeneratorListener.cs example file
	// note: this is an example for the Windows Standalone platform only!
	public class GenuineValidatorExample : MonoBehaviour
	{
		// 💖 looks like a really lovely separator =)
		// used at CodeHashGeneratorListener
		public const string Separator = "💖";
		
		// let's choose some non-obvious file name which will not be hashed (not .dll or .exe)
		// used at CodeHashGeneratorListener
		public const string FileName = "Textures.asset";
		
		// used at CodeHashGeneratorListener
		public static readonly char[] StringKey = {'\x674', '\x345', '\x856', '\x968', '\x322'};
		
		private string status = "Press Check";

		// just an unoptimized example of SHA1 hashing
		public static string GetHash(string firstBuildHash)
		{
			var stringBytes = StringUtils.StringToBytes(firstBuildHash);
			using (var sha1 = new SHA1Wrapper())
			{
				var hash = sha1.ComputeHash(stringBytes);
				return StringUtils.HashBytesToHexString(hash);
			}
		}
		
		private void OnGUI()
		{
			if (!CodeHashGenerator.Instance.IsBusy)
			{
				if (GUILayout.Button("Check"))
					OnCheckHashClick();
			}
			
			GUILayout.Label(status);
		}

		private async void OnCheckHashClick()
		{
			status = "Checking...";
			try
			{
				var result = await CodeHashGenerator.GenerateAsync();
				OnGotHash(result);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private void OnGotHash(HashGeneratorResult result)
		{
			if (!result.Success)
			{
				status = "Error: " + result.ErrorMessage;
				return;
			}

			var filePath = Path.Combine(Path.GetFullPath(Application.dataPath + @"\..\"), FileName);
			if (!File.Exists(filePath))
			{
				status = "No super secret file found, you're cheater!\n" + filePath + "\nSummaryHash: " + result.SummaryHash;
				return;
			}

			var allBytes = File.ReadAllBytes(filePath);
			var allChars = BytesToUnicodeChars(allBytes);
			var decrypted = ObscuredString.Decrypt(allChars, StringKey);

			var separatorIndex = decrypted.IndexOf(Separator, StringComparison.InvariantCulture);
			if (separatorIndex == -1)
			{
				status = "Super secret file is corrupted, you're cheater!\nSummaryHash: " + result.SummaryHash;
				return;
			}

			var whitelistedHashes = decrypted.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
			var originalSummaryHash = whitelistedHashes[0];

			// compare summary hashes first
			if (originalSummaryHash != result.SummaryHash)
			{
				// check all files against whitelisted hashes if summary differs
				// (it can differ if some files are absent due to build separation)
				for (var i = 1; i < whitelistedHashes.Length; i++)
				{
					var hash = whitelistedHashes[i];
					if (!result.HasFileHash(hash))
					{
						status = "Code hash differs, you're cheater!\nSummary hashes:\n" + originalSummaryHash + "\n" + result.SummaryHash + "\nWhitelisted hashes count: " + whitelistedHashes.Length;
						return;
					}
				}
			}

			status = "All fine!\nSummaryHash: " + result.SummaryHash;
		}

		public static char[] BytesToUnicodeChars(byte[] input)
		{
			var chars = new char[input.Length / sizeof(char)];
			Buffer.BlockCopy(input, 0, chars, 0, input.Length);
			return chars;
		}

		public static byte[] UnicodeCharsToBytes(char[] input)
		{
			var bytes = new byte[input.Length * sizeof(char)];
			Buffer.BlockCopy(input, 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}