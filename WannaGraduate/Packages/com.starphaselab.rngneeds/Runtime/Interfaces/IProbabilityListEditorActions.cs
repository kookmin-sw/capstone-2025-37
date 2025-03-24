#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds
{
    [Obsolete("Used only by RNGNeeds ProbabilityList Drawer")]
    public interface IProbabilityListEditorActions
    {
        Type ItemType { get; }
        ItemProviderType GetItemProviderTypes(int index);
        TestResults RunTest();
        List<Vector2> GetInfluencedProbabilitiesLimits { get; }
        
        int IndexOfUnremovableItem { get; }
        int UnlockedItemsCount { get; }
        bool IsListInfluenced { get; }
        string SelectionMethodID { get; set; }

        void AddDefaultItem();
        void AdjustItemBaseProbability(int index, float amount);
        void ResetAllProbabilities();
        bool RemoveItemAtIndex(int index, bool normalize = true);
        bool SetItemBaseProbability(int index, float probability, bool normalize = true);
        float GetItemBaseProbability(int index);
        float NormalizeProbabilities();
    }
}
#endif