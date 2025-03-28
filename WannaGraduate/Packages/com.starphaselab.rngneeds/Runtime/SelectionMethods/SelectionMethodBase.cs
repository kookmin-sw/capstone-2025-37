using Unity.Collections;
using Random = Unity.Mathematics.Random;

namespace RNGNeeds
{
    /// <summary>
    /// Base class for building custom selection methods in RNGNeeds. Provides a partial implementation of the <see cref="ISelectionMethod"/>.
    /// Use this as a starting point for your own selection methods, or build a full implementation of the interface.
    /// </summary>
    public abstract class SelectionMethodBase : ISelectionMethod
    {
        public abstract string Identifier { get; }
        public abstract string Name { get; }

        #if UNITY_EDITOR
        public virtual string EditorTooltip => string.Empty;
        #endif
        
        protected IProbabilityList ProbabilityList;
        protected NativeArray<ItemData> ItemData;
        protected NativeList<int> EnabledIndicesWithPositiveProbability;
        protected NativeList<int> AllIndicesWithPositiveProbability;

        protected int LastPickedIndex;
        protected int ItemCount;
        protected Random m_Random;
        protected float TotalProbability;

        private void PrepItemInternal(IProbabilityItem probabilityItem, int index)
        {
            var isEnabled = false;
            var hasPositiveProbability = false;
            
            if (probabilityItem.Probability > 0f)
            {
                hasPositiveProbability = true;
                AllIndicesWithPositiveProbability.Add(index);
                if (probabilityItem.Enabled)
                {
                    isEnabled = true;
                    EnabledIndicesWithPositiveProbability.Add(index);
                }
                
                TotalProbability += probabilityItem.Probability;    
            }
            
            PrepItem(probabilityItem, index, isEnabled, hasPositiveProbability);
        }
        
        protected virtual void Prep(IProbabilityList probabilityList)
        {
            ProbabilityList = probabilityList;
            ItemCount = probabilityList.ItemCount;
            LastPickedIndex = ProbabilityList.LastPickedIndex;
            ItemData = SelectionTools.GetItemData(probabilityList, Allocator.Temp);
            
            EnabledIndicesWithPositiveProbability = new NativeList<int>(Allocator.Temp);
            AllIndicesWithPositiveProbability = new NativeList<int>(Allocator.Temp);
            
            TotalProbability = 0f;
            
            for (var i = 0; i < ItemCount; i++)
            {
                PrepItemInternal(ProbabilityList.Item(i), i);
            }
        }

        protected virtual void PrepItem(IProbabilityItem probabilityItem, int index, bool isEnabled, bool hasPositiveProbability) { }

        public NativeList<int> SelectItems(IProbabilityList probabilityList, int pickCount)
        {
            Prep(probabilityList);
            
            var shouldPreventRepeatsInLoop = 
                (ProbabilityList.PreventRepeat == PreventRepeatMethod.Spread || ProbabilityList.PreventRepeat == PreventRepeatMethod.Repick)
                && EnabledIndicesWithPositiveProbability.Length > 1;
            
            var pickedIndices = new NativeList<int>(Allocator.Temp);
            if (AllIndicesWithPositiveProbability.Length == 0)
            {
                Clean();
                return pickedIndices;
            }
            
            m_Random = new Random(ProbabilityList.CurrentSeed);
            
            for (var i = 0; i < pickCount; i++)
            {
                if (SelectItem(out var selectedIndex) == false)
                {
                    if (ProbabilityList.MaintainPickCountIfDisabled) i--;
                    continue;
                }
            
                if (shouldPreventRepeatsInLoop && LastPickedIndex == selectedIndex)
                {
                    switch (ProbabilityList.PreventRepeat)
                    {
                        case PreventRepeatMethod.Spread:
                            selectedIndex = SpreadResult(selectedIndex);
                            break;
                        case PreventRepeatMethod.Repick:
                            i--;
                            continue;
                    }
                }
            
                LastPickedIndex = selectedIndex;
                pickedIndices.Add(selectedIndex);
            }
            
            if (ProbabilityList.PreventRepeat == PreventRepeatMethod.Shuffle && pickedIndices.Length > 1) ShuffleResult(pickedIndices, ProbabilityList.ShuffleIterations);
            Clean();
            return pickedIndices;
        }

        protected virtual void Clean()
        {
            EnabledIndicesWithPositiveProbability.Dispose();
            AllIndicesWithPositiveProbability.Dispose();
        }
        
        protected virtual int SpreadResult(int index)
        {
            return SelectionTools.SpreadResult(index, EnabledIndicesWithPositiveProbability, m_Random);
        }

        protected virtual void ShuffleResult(NativeList<int> indices, int iterations)
        {
            SelectionTools.ShuffleResult(indices, iterations, m_Random);
        }
        
        protected abstract bool SelectItem(out int selectedIndex);
    }
}