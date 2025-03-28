namespace RNGNeeds
{
    /// <summary>
    /// The ISeedProvider interface provides a mechanism for generating seeds for random number generation.
    /// Implement this interface to use your own custom seed provider through <see cref="RNGNeedsCore.SetSeedProvider"/>.
    /// </summary>
    public interface ISeedProvider
    {
        uint NewSeed { get; }
    }
}