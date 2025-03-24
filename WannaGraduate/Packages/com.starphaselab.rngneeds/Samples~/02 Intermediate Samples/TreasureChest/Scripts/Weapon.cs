using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "RNGNeeds/Treasure Chest/Weapon")]
    public class Weapon : Item
    {
        public int damage;
    }
}