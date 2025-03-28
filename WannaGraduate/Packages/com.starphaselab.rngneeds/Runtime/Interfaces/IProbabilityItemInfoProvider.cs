namespace RNGNeeds
{
    /// <summary>
    /// Interface for providing additional item information.
    /// This is primarily used for the inspector drawer in Unity's Editor, 
    /// but can also be utilized during runtime.
    /// </summary>
    public interface IProbabilityItemInfoProvider
    {
        string ItemInfo { get; }
    }
}