#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal abstract class BaseWorker
	{
		public HashGeneratorResult Result { get; private set; }
		public bool IsBusy { get; private set; }
		
		protected readonly int threadsCount;
		
		public BaseWorker(int threadsCount)
		{
			this.threadsCount = threadsCount;
		}

		public virtual void Execute()
		{
			IsBusy = true;
		}

		protected virtual void Complete(HashGeneratorResult result)
		{
			Result = result;
			IsBusy = false;
		}
	}
}