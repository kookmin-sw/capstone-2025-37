#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System.Threading.Tasks;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	public delegate void HashGeneratorResultHandler(HashGeneratorResult result);

	/// <summary>
	/// CodeHashGenerator interface to make it easier to use it through the Instance.
	/// </summary>
	public interface ICodeHashGenerator
	{
		/// <summary>
		/// Stores previously calculated result.
		/// Can be null if Generate() wasn't called yet or if it was called but calculation is still in process.
		/// </summary>
		/// \sa #IsBusy
		HashGeneratorResult LastResult { get; }

		/// <summary>
		/// Indicates if hash generation is currently in process.
		/// </summary>
		bool IsBusy { get; }

		/// <summary>
		/// Call to start current runtime code hash generation. Automatically adds instance to the scene if necessary.
		/// </summary>
		/// <param name="maxThreads">Threads to use while hashing files. Use Environment.ProcessorCount to use all cores.</param>
		ICodeHashGenerator Generate(int maxThreads = 1);
		
		/// <summary>
		/// Awaitable version of Generate(). Allows awaiting for the generation result.
		/// </summary>
		/// <param name="maxThreads">Threads to use while hashing files. Use Environment.ProcessorCount to use all cores.</param>
		Task<HashGeneratorResult> GenerateAsync(int maxThreads = 1);
	}
}