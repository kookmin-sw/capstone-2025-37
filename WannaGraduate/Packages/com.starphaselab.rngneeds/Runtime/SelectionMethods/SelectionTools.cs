using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace RNGNeeds
{
    public struct ItemData
    {
        public readonly int Index;
        public readonly bool Enabled;
        public readonly float Probability;

        public ItemData(int index, bool enabled, float probability)
        {
            Index = index;
            Enabled = enabled;
            Probability = probability;
        }
    }

    internal static class SelectionTools
    {
        public static int SpreadResult(int index, NativeList<int> spreadMap, Random random)
        {
            var currentIndex = spreadMap.IndexOf(index);
            if (currentIndex == -1) return index;
            
            var chooseNext = random.NextFloat() > 0.5f;

            if (chooseNext) index = spreadMap[(currentIndex + 1) % spreadMap.Length];
            else index = spreadMap[(currentIndex - 1 + spreadMap.Length) % spreadMap.Length];
        
            return index;
        }
        
        public static void ShuffleResult(NativeList<int> indices, int iterations, Random random)
        {
            var hasTwoDistinctIndices = false;
            for (var i = 1; i < indices.Length; i++)
            {
                if (indices[i] == indices[i - 1]) continue;
                hasTwoDistinctIndices = true;
                break;
            }
            
            if (hasTwoDistinctIndices == false) return;
            
            for(var iteration = 1; iteration <= iterations; iteration++)
            {
                for (var i = 1; i < indices.Length; i++)
                {
                    if (indices[i] != indices[i - 1]) continue;
                    int randomIndex;
                    do
                    {
                        randomIndex = random.NextInt(0, indices.Length);
                    } while (randomIndex == i || randomIndex == i - 1 || indices[randomIndex] == indices[i]);
        
                    (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
                }
            }
        }
        
        public static int BinarySearch(NativeArray<float> array, float target, float epsilon = 1e-6f)
        {
            var left = 0;
            var right = array.Length - 1;

            while (left <= right)
            {
                var middle = left + ((right - left) / 2);

                if (math.abs(array[middle] - target) < epsilon) return middle;

                if (array[middle] < target) left = middle + 1;
                else right = middle - 1;
            }

            return left;
        }

        public static int LinearSearch(float randomValue, NativeArray<ItemData> itemData)
        {
            var cumulativeProbability = 0f;

            foreach (var item in itemData)
            {
                if(item.Probability <= 0f) continue;
                cumulativeProbability += item.Probability;
                if (randomValue <= cumulativeProbability == false) continue;
                return item.Index;
            }
        
            // execution should never reach this point
            return -1;
        }

        public static NativeArray<ItemData> GetItemData(IProbabilityList probabilityList, Allocator allocator)
        {
            var itemsInList = probabilityList.ItemCount;
            var items = new NativeArray<ItemData>(itemsInList, allocator);
            
            for (var i = 0; i < itemsInList; i++)
            {
                var probabilityItem = probabilityList.Item(i);
                items[i] = new ItemData(i, probabilityItem.Enabled, probabilityItem.Probability);
            }

            return items;
        }
    }
}