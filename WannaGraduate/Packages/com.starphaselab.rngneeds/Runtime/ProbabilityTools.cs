using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StarphaseTools.Core;
using Unity.Collections;
using UnityEngine;

namespace RNGNeeds
{
    internal static class ProbabilityTools
    {
        private static readonly List<float> m_MinProbabilities = new List<float>();
        private static readonly List<float> m_MaxProbabilities = new List<float>();
        private static readonly List<Vector2> m_InfluenceLimits = new List<Vector2>();
        
        public static List<Vector2> GetInfluencedProbabilitiesLimits<T>(this List<ProbabilityItem<T>> ProbabilityItems)
        {
            m_MinProbabilities.Clear();
            m_MaxProbabilities.Clear();
            m_InfluenceLimits.Clear();
            
            foreach (var item in ProbabilityItems)
            {
                m_MinProbabilities.Add(item.GetInfluencedProbability(-1f));
                m_MaxProbabilities.Add(item.GetInfluencedProbability(1f));
            }
            
            NormalizeList(m_MinProbabilities);
            NormalizeList(m_MaxProbabilities);
            
            for (var i = 0; i < ProbabilityItems.Count; i++)
            {
                m_InfluenceLimits.Add(new Vector2(m_MinProbabilities[i], m_MaxProbabilities[i]));
            }
            
            return m_InfluenceLimits;
        }
        
        internal static float CalculateInfluencedProbability(float probability, float spreadLow, float spreadHigh, float influence)
        {
            var positiveRange = spreadHigh - probability;
            var negativeRange = probability - spreadLow;

            var result = probability + (influence > 0 ? positiveRange * influence : negativeRange * influence);
            
            return Mathf.Clamp(result, spreadLow, spreadHigh);
        }

        private static void NormalizeList(IList<float> list)
        {
            var sum = list.Sum();

            for (var i = 0; i < list.Count; i++)
            {
                var adjusted = list[i] / sum;
                list[i] = float.IsNaN(adjusted) || adjusted < .0000001f ? 0f : adjusted;
            }
        }

        // public static float TotalEnabledProbability<T>(this ProbabilityList<T> list)
        // {
        //     var totalEnabledProbability = 0f;
        //     foreach (var probabilityItem in list.ProbabilityItems)
        //     {
        //         if (probabilityItem.Enabled) totalEnabledProbability += probabilityItem.Probability;
        //     }
        //
        //     return totalEnabledProbability;
        // }

        public static TestResults Test<T>(this ProbabilityList<T> list)
        {
            var pickCount = list.PickCountCurve.PickRandomInt(list.PickCountMin, list.PickCountMax, list.Seed);
            var selectionMethod = list.SelectionMethodInstance;
            
            var testResults = new TestResults
            {
                pickCount = pickCount,
                selectionMethodName = selectionMethod.Name,
                preventRepeat = list.PreventRepeat,
            };
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            list.SelectItemsInternal(pickCount, selectionMethod, out var selection);
            testResults.seed = list.CurrentSeed;
            
            stopwatch.Stop();
            testResults.testDuration = stopwatch.Elapsed.TotalMilliseconds;

            if(selection.Length > 0)
            {
                list.LastPickCount = selection.Length;
                for (var i = 0; i < list.ProbabilityItems.Count; i++) testResults.indexPicks.Add(i, 0);
                foreach (var pick in selection) testResults.indexPicks[pick]++;
            }
            else list.LastPickCount = 0;
            
            testResults.repeatCount = CountRepeats(selection);
            testResults.actualPickCount = selection.Length;
            testResults.pickCountMin = list.PickCountMin;
            testResults.pickCountMax = list.PickCountMax;
            testResults.Print();

            selection.Dispose();
            return testResults;
        }

        private static int CountRepeats(NativeList<int> indices)
        {
            var repeats = 0;
            for (var i = 1; i < indices.Length; i++)
            {
                if (indices[i] != indices[i - 1]) continue;
                repeats++;
            }

            return repeats;
        }
    }
}