using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Gold Pile", menuName = "RNGNeeds/Treasure Chest/Gold Pile")]
    public class GoldPile : Item
    {
        [Header("Gold Pile Setup")]
        public int baseAmount;
        public ProbabilityList<float> multiplier;
        
        [Header("Generated Gold Amount")]
        public int amount;
        
        public override void ItemSetup(int playerLevel)
        {
            amount = Mathf.RoundToInt(baseAmount + Random.Range(1, playerLevel) * playerLevel * multiplier.PickValue()) + Random.Range(1, 10);
        }

        public override string ItemDescription => $"{title} ({amount.ToString()})";
    }
}