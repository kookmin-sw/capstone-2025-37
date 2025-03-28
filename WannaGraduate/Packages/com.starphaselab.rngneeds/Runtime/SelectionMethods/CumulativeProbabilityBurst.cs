using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

namespace RNGNeeds
{
    internal class CumulativeProbabilityBurst : ISelectionMethod
    {
        public string Identifier => "CPMBurst";
        public string Name => "Cumulative Probability [BURST]";
        
        #if UNITY_EDITOR
        public string EditorTooltip => "A cumulative probability method using binary search. Offers good performance across various list sizes. Burst-compiled Job version, great for selecting large number of items in one pick.";
        #endif
        
        public NativeList<int> SelectItems(IProbabilityList probabilityList, int pickCount)
        {
            var _pickedIndices = new NativeList<int>(pickCount, Allocator.TempJob);
            
            var job = new CumulativeBurstSelectJob()
            {
                lastPickedIndices = _pickedIndices,
                ItemData = SelectionTools.GetItemData(probabilityList, Allocator.TempJob),
                JobData = SelectJobUtility.GetJobData(probabilityList, pickCount)
            };
                
            job.Prep();
            var handle = job.Schedule();
            handle.Complete();
            job.Clean();

            return _pickedIndices;
        }

        [BurstCompile(FloatPrecision.High, FloatMode.Strict)]
        private struct CumulativeBurstSelectJob : IJob
        {
            public NativeList<int> lastPickedIndices;
            public NativeArray<ItemData> ItemData;
            public SelectJobData JobData;
            
            private NativeList<int> m_EnabledItemIndicesWithPositiveProb;
            private NativeList<int> m_ItemIndicesWithPositiveProb;

            private float totalProbability;
            
            private NativeList<float> m_CumulativeProbabilities;

            public void Prep()
            {
                m_ItemIndicesWithPositiveProb = new NativeList<int>(JobData.itemCount, Allocator.TempJob);
                m_EnabledItemIndicesWithPositiveProb = new NativeList<int>(JobData.itemCount, Allocator.TempJob);
                m_CumulativeProbabilities = new NativeList<float>(JobData.itemCount, Allocator.TempJob);
                
                totalProbability = 0f;
                
                for (var i = 0; i < JobData.itemCount; i++)
                {
                    var probabilityItem = ItemData[i];
                    if (probabilityItem.Probability <= 0f) continue;
                    totalProbability += probabilityItem.Probability;
                    m_CumulativeProbabilities.Add(totalProbability);
                    m_ItemIndicesWithPositiveProb.Add(i);
                    if (probabilityItem.Enabled == false) continue;
                    m_EnabledItemIndicesWithPositiveProb.Add(i);
                }
            }

            public void Execute()
            {
                if (m_ItemIndicesWithPositiveProb.Length <= 0) return;
                
                var shouldPreventRepeatsInLoop = 
                    (JobData.preventRepeatMethod == PreventRepeatMethod.Spread || JobData.preventRepeatMethod == PreventRepeatMethod.Repick)
                    && m_EnabledItemIndicesWithPositiveProb.Length > 1;

                var random = new Random(JobData.seed);
                
                for (var i = 0; i < JobData.pickCount; i++)
                {
                    var index = SelectionTools.BinarySearch(m_CumulativeProbabilities, random.NextFloat(0f, totalProbability));

                    index = m_ItemIndicesWithPositiveProb[index];
                    if (ItemData[index].Enabled == false)
                    {
                        if (JobData.maintainPickCountWhenDisabled) i--;
                        continue;
                    }
                    
                    if (shouldPreventRepeatsInLoop && JobData.lastPickedIndex == index)
                    {
                        switch (JobData.preventRepeatMethod)
                        {
                            case PreventRepeatMethod.Spread:
                                index = SelectionTools.SpreadResult(index, m_EnabledItemIndicesWithPositiveProb, random);
                                break;
                            case PreventRepeatMethod.Repick:
                                i--;
                                continue;
                        }
                    }

                    JobData.lastPickedIndex = index;
                    lastPickedIndices.Add(index);
                }
                
                if(JobData.preventRepeatMethod == PreventRepeatMethod.Shuffle && m_EnabledItemIndicesWithPositiveProb.Length > 1) SelectionTools.ShuffleResult(lastPickedIndices, JobData.shuffleIterations, random);
            }

            public void Clean()
            {
                ItemData.Dispose();
                m_ItemIndicesWithPositiveProb.Dispose();
                m_EnabledItemIndicesWithPositiveProb.Dispose();
                m_CumulativeProbabilities.Dispose();
            }
        }
    }
}