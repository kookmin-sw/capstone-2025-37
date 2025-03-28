namespace RNGNeeds
{
#pragma warning disable 0618
    public interface IProbabilityList
        #if UNITY_EDITOR
        : IProbabilityListEditorActions
        #endif
    {
        IProbabilityItem Item(int index);
        PreventRepeatMethod PreventRepeat { get; }
        bool MaintainPickCountIfDisabled { get; }
        int ItemCount { get; }
        int LastPickedIndex { get; }
        int ShuffleIterations { get; }
        uint CurrentSeed { get; }
        uint Seed { get; }
    }
#pragma warning restore 0618
}