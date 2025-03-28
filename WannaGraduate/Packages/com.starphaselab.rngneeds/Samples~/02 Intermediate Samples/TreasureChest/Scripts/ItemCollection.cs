using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Item Collection", menuName = "RNGNeeds/Treasure Chest/Item Collection")]
    public class ItemCollection : ScriptableObject, IProbabilityItemColorProvider
    {
        public ItemRarity rarity;
        public Color ItemColor => rarity == null ? Color.magenta : rarity.rarityColor;
        public ProbabilityList<Item> items;

        public List<Item> PickItems()
        {
            return items.PickValues();
        }
    }
}