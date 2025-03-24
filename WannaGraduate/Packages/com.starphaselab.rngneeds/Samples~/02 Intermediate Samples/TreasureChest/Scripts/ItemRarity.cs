using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Item Rarity", menuName = "RNGNeeds/Treasure Chest/Item Rarity")]
    public class ItemRarity : ScriptableObject, IProbabilityItemColorProvider, IProbabilityItemInfoProvider
    {
        public string title;
        public Color rarityColor;
        public Color ItemColor => rarityColor;
        public string ItemInfo => title;
    }
}