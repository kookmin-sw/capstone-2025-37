using System;

namespace RNGNeeds
{
    /// <summary>
    /// The DefaultSeedProvider is an implementation of the <see cref="ISeedProvider"/> interface that generates
    /// seeds based on the current time and a System.Random object. This is the default provider used by the system, 
    /// but you can replace it with your own implementation if needed.
    /// </summary>
    public class DefaultSeedProvider : ISeedProvider
    {
        private readonly System.Random _random = new System.Random();
        private readonly object _lockObject = new object();

        public uint NewSeed
        {
            get
            {
                lock (_lockObject)
                {
                    return (uint)(_random.Next() * DateTime.UtcNow.Ticks % uint.MaxValue);
                }
            }
        }
    }
}