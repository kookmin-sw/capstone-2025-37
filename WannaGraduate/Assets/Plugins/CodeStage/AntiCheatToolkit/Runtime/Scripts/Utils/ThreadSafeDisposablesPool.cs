#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;

namespace CodeStage.AntiCheat.Utils
{
	public class ThreadSafeDisposablesPool<T> where T : IDisposable, new()
	{
		private readonly ConcurrentBag<T> objects;
		private readonly Func<T> objectGenerator;

		public ThreadSafeDisposablesPool(Func<T> objectGenerator)
		{
			this.objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
			objects = new ConcurrentBag<T>();
		}

		public T Get()
		{
			return objects.TryTake(out T item) ? item : objectGenerator();
		}

		public void Release(T item)
		{
			objects.Add(item);
		}
		
		public void Dispose()
		{
			while (objects.TryTake(out T item))
			{
				item.Dispose();
			}
		}
	}
}