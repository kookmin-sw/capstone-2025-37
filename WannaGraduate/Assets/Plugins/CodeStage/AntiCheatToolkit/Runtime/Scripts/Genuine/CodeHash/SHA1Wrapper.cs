#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	
	/// <summary>
	/// Just an Utility class to make it easier to work with SHA1.
	/// </summary>
	/// Not intended for usage from user code,
	/// touch at your peril since API can change and break backwards compatibility!
	public class SHA1Wrapper : IDisposable
	{
		private readonly SHA1Managed sha1;

		public SHA1Wrapper()
		{
			sha1 = new SHA1Managed();
		}

		public byte[] ComputeHash(Stream stream)
		{
			return sha1.ComputeHash(stream);
		}

		public byte[] ComputeHash(byte[] bytes)
		{
			return sha1.ComputeHash(bytes);
		}

		public void Dispose()
		{
			sha1?.Dispose();
		}
	}
}