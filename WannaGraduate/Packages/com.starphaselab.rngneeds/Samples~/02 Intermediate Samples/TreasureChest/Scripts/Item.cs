using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    public class Item : ScriptableObject, IProbabilityItemColorProvider
    {
        public string title;
        public ItemRarity rarity;
        public virtual Color ItemColor => rarity == null ? Color.magenta : rarity.ItemColor;

        public virtual string ItemDescription => $"{title} ({rarity.title} {GetType().Name})";

        public virtual void ItemSetup(int playerLevel) { }
    }
}