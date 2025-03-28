namespace RNGNeeds
{
    /// <summary>
    /// Interface for providing influence information and the influence value itself.
    /// This is used to modify the base probability of an item during the selection process,
    /// allowing for dynamic adjustments.
    /// </summary>
    public interface IProbabilityInfluenceProvider
    {
        string InfluenceInfo { get; }
        float ProbabilityInfluence { get; }
    }
}