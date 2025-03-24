using Unity.Collections;

namespace RNGNeeds
{
    /// <summary>
    /// An interface for creating a way to select items from a probability list. If you're looking to create your own 
    /// unique way to pick items from the list, this is the interface you'll want to implement. Each selection method 
    /// comes with a unique identifier and a name for easy reference.
    /// </summary>
    public interface ISelectionMethod
    {
        string Identifier { get; }
        string Name { get; }
        #if UNITY_EDITOR
        string EditorTooltip { get; }
        #endif
        NativeList<int> SelectItems(IProbabilityList probabilityList, int pickCount);
    }
}