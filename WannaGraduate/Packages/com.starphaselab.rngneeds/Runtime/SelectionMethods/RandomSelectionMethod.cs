namespace RNGNeeds
{
    internal class RandomSelectionMethod : SelectionMethodBase
    {
        public override string Identifier => "RSMRegular";
        public override string Name => "Random Selection";

        #if UNITY_EDITOR
        public override string EditorTooltip => "Probability distribution will be ignored, and results will be pure random. Useful when a uniform distribution of items is desired.";
        #endif
        
        protected override bool SelectItem(out int selectedIndex)
        {
            var item = ItemData[m_Random.NextInt(0, ItemData.Length)];
            selectedIndex = item.Index;
            return item.Enabled;
        }
    }
}