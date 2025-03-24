#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeStage.AntiCheat.Common;
using Debug = UnityEngine.Debug;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	/// <summary>
	/// Contains hashes for the application build.
	/// </summary>
	public class BuildHashes
	{
		/// <summary>
		/// Path to the build file or folder.
		/// </summary>
		public string BuildPath { get; }

		/// <summary>
		/// Contains all sensitive files hashes and relative paths.
		/// </summary>
		public IReadOnlyList<FileHash> FileHashes { get; }

		/// <summary>
		/// Summary hash for all files in build.
		/// </summary>
		/// Use with caution: summary hash for runtime build may differ from the summary hash
		/// you got in Editor, for example, for Android App Bundles.
		/// Use #FileHashes for more accurate hashes comparison control.
		public string SummaryHash { get; }

		/// <summary>
		/// Hashing duration in seconds. Will be 0 if hashing was not succeed.
		/// </summary>
		public double DurationSeconds { get; set; }
		
		internal BuildHashes(string buildPath, FileHash[] fileHashes)
		{
			Array.Sort(fileHashes, (x, y) => string.Compare(x.Hash, y.Hash, StringComparison.Ordinal));

			BuildPath = buildPath;
			SummaryHash = CalculateSummaryCodeHash(fileHashes);
			FileHashes = fileHashes;
		}

		internal BuildHashes(string buildPath, FileHash[] fileHashes, string summaryHash)
		{
			BuildPath = buildPath;
			SummaryHash = summaryHash;
			FileHashes = fileHashes;
		}

		/// <summary>
		/// Checks is passes hash exists in file hashes of this instance.
		/// </summary>
		/// <param name="hash">Target file hash.</param>
		/// <returns>True if such hash presents at #FileHashes and false otherwise.</returns>
		public bool HasFileHash(string hash)
		{
			foreach (var fileHash in FileHashes)
			{
				if (fileHash.Hash == hash)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sends enclosing hashes to the console along with file names.
		/// </summary>
		public void PrintToConsole()
		{
			var log = ACTk.LogPrefix + $"Build hashed ({DurationSeconds:F2} sec, {FileHashes?.Count} files): " + BuildPath + "\n";

			if (!Path.GetExtension(BuildPath).Equals(".aab", StringComparison.OrdinalIgnoreCase))
			{
				log += "Summary Hash: " + SummaryHash + "\n";
			}
			else
			{
#if UNITY_EDITOR
				var warningPrefix = "<b>[Warning]</b> ";
#else
				var warningPrefix = "[Warning] ";
#endif
				log += warningPrefix + "App Bundle Summary Hash will more likely " +
					   "differ from the Summary Hash you'll get at runtime on target devices.\n" +
					   "Please use individual File Hashes instead.\n";
			}

			if (FileHashes != null)
			{
				log += "Individual File Hashes:";
			         
				foreach (var fileHash in FileHashes)
				{
					log += "\n" + fileHash.Path + " : " + fileHash.Hash;
				}
			}

			Debug.Log(log);
		}

		private string CalculateSummaryCodeHash(FileHash[] hashes)
		{
			var hashesString = string.Empty;
			if (hashes == null) return hashesString;
			var count = hashes.Length;
			if (count == 0) return hashesString;

			var hashLength = hashes[0].Hash.Length;

			var hashBytes = new byte[count][];
			for (var i = 0; i < count; i++)
			{
				hashBytes[i] = Encoding.UTF8.GetBytes(hashes[i].Hash);
			}

			var averageHashBytes = new byte[hashLength / 2];
			for (var i = 0; i < hashLength; i += 2)
			{
				byte result = 0;
				for (var j = 0; j < count; j++)
				{
					var b1 = hashBytes[j][i];
					var b2 = hashBytes[j][i + 1];
					result ^= (byte)((b1 << 4) | b2);
				}
				averageHashBytes[i / 2] = result;
			}

			var sb = new StringBuilder();
			foreach (var b in averageHashBytes)
			{
				sb.Append(b.ToString("x2"));
			}
			
			return sb.ToString().ToUpperInvariant().Substring(0, hashLength);
		}
	}
}