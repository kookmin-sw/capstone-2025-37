using UnityEngine;

namespace RNGNeeds
{
    /// <summary>
    /// Interface for providing a color associated with an item.
    /// This is primarily used for the inspector drawer in Unity's Editor,
    /// but can also be utilized during runtime for example to color-code items.
    /// </summary>
    public interface IProbabilityItemColorProvider
    {
        Color ItemColor { get; }
    }
}