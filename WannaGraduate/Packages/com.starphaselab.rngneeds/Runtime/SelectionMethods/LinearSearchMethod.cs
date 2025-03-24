namespace RNGNeeds
{
    internal class LinearSearchMethod : SelectionMethodBase
    {
        public override string Identifier => "LSMRegular";
        public override string Name => "Linear Search";

        #if UNITY_EDITOR
        public override string EditorTooltip => "A simple linear search method. Fast for small lists, but performance degrades as the list size grows. Ideal for game development (frequent low number of picks from smaller lists).";
        #endif

        protected override bool SelectItem(out int selectedIndex)
        {
            selectedIndex = SelectionTools.LinearSearch(m_Random.NextFloat(0f, TotalProbability), ItemData);
            return ItemData[selectedIndex].Enabled;
        }
    }
}