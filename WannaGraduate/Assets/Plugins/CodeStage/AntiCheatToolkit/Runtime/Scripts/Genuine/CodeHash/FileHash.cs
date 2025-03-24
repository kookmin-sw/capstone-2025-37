#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System.IO;
using CodeStage.AntiCheat.Utils;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	/// <summary>
	/// Holds hash for the specific file.
	/// </summary>
	public class FileHash
	{
		/// <summary>
		/// Path to the file which was hashed.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// Hash of the file. Calculated using semi-custom hashing algorithm based on SHA1.
		/// </summary>
		public string Hash { get; }

		internal FileHash(string path, Stream stream, ThreadSafeDisposablesPool<SHA1Wrapper> sha1Pool)
		{
			Path = path;
			var sha1 = sha1Pool.Get();
			var hash = sha1.ComputeHash(stream);
			sha1Pool.Release(sha1);
			Hash = StringUtils.HashBytesToHexString(hash);
		}
		
		internal FileHash(string path, string hash)
		{
			Path = path;
			Hash = hash;
		}

		public override string ToString()
		{
			return Path + ": " + Hash;
		}
	}
}