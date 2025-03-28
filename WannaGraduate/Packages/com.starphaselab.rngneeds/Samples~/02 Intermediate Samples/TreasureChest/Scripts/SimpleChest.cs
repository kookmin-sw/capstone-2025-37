using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Simple Chest", menuName = "RNGNeeds/Treasure Chest/Simple Chest")]
    public class SimpleChest : ChestBase
    {
        public ProbabilityList<ItemCollection> weaponsCollection;
        public ProbabilityList<Item> armor;
        public ProbabilityList<Item> craftingMaterials;
        public ProbabilityList<Item> potions;
        
        public override List<Item> OpenChest()
        {
            var pickedItems = new List<Item>();
            var pickedWeaponsCollections = weaponsCollection.PickValues();
            foreach (var pickedWeaponsCollection in pickedWeaponsCollections)
            {
                pickedItems.AddRange(pickedWeaponsCollection.PickItems());
            }
            
            pickedItems.AddRange(armor.PickValues());
            pickedItems.AddRange(craftingMaterials.PickValues());
            pickedItems.AddRange(potions.PickValues());
            
            for (var i = pickedItems.Count - 1; i >= 0; i--)
            {
                var pickedItem = pickedItems[i];
                if (pickedItem == null) pickedItems.RemoveAt(i);
                var itemInstance = Instantiate(pickedItem);
                itemInstance.ItemSetup(playerLevel);
                pickedItems[i] = itemInstance;
            }

            return pickedItems;
        }
    }
}