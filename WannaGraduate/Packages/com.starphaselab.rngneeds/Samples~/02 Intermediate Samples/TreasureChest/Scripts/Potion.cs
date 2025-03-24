using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "RNGNeeds/Treasure Chest/Potion")]
    public class Potion : Item
    {
        [Header("Potion Setup")]
        public Color potionColor;
        public PlayerStat playerStat;
        public int baseAmount;
        public int levelMultiplier;
        
        [Header("Generated Amount")]
        public int amount;
        
        public override string ItemDescription => $"{title} ({amount.ToString()} {playerStat})";
        public override Color ItemColor => potionColor;

        public override void ItemSetup(int playerLevel)
        {
            amount = Mathf.RoundToInt(baseAmount + playerLevel * levelMultiplier);
        }
    }

    public enum PlayerStat
    {
        Health,
        Mana,
        Stamina
    }
}