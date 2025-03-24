using Unity.Collections;

namespace RNGNeeds
{
    internal class CumulativeProbability : SelectionMethodBase
    {
        public override string Identifier => "CPMRegular";
        public override string Name => "Cumulative Probability";

        #if UNITY_EDITOR
        public override string EditorTooltip => "A cumulative probability method using binary search. Offers good performance across various list sizes.";
        #endif
        
        private NativeList<float> m_CumulativeProbabilities;
        
        protected override void Prep(IProbabilityList probabilityList)
        {
            m_CumulativeProbabilities = new NativeList<float>(Allocator.Temp);
            base.Prep(probabilityList);
        }

        protected override void PrepItem(IProbabilityItem probabilityItem, int index, bool isEnabled, bool hasPositiveProbability)
        {
            if(hasPositiveProbability) m_CumulativeProbabilities.Add(TotalProbability);
        }
        
        protected override bool SelectItem(out int selectedIndex)
        {
            var index = SelectionTools.BinarySearch(m_CumulativeProbabilities, m_Random.NextFloat(0f, TotalProbability));
            selectedIndex = AllIndicesWithPositiveProbability[index];
            return ItemData[selectedIndex].Enabled;
        }

        protected override void Clean()
        {
            base.Clean();
            m_CumulativeProbabilities.Dispose();
        }
    }
}