using UnityEngine;

namespace RNGNeeds.Editor
{
    internal class ItemInfoCache
    {
        public string index;
        public string info;
        public string stripePercentage;
        public string listElementPercentage;
        public string spreadMinPercentage;
        public string spreadMaxPercentage;
        public float influenceInfoHeight;
        public bool isExpandedProperty;
        
        public Vector2 InfluencedProbabilityLimits;
        public bool InfluenceProviderExpanded;
        public ItemProviderType ProviderType;
        public object valueObject;
        public IProbabilityItem probabilityItemObject;
    }
}