using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    [CreateAssetMenu(fileName = "New Chest", menuName = "RNGNeeds/Treasure Chest/Chest")]
    public class Chest : ChestBase
    {
        public PLCollection<ItemCollection> chestLootTable;

        public override List<Item> OpenChest()
        {
            var selectedCollections = chestLootTable.PickFromAll();

            chestContents = new List<Item>();

            foreach (var collection in selectedCollections)
            {
                if(collection == null) continue;
                chestContents.AddRange(collection.PickItems());
            }
            
            var droppedItems = new List<Item>();
            foreach (var droppedItem in chestContents)
            {
                if (droppedItem == null) continue;
                var itemInstance = Instantiate(droppedItem);
                itemInstance.ItemSetup(playerLevel);
                droppedItems.Add(itemInstance);
            }

            return droppedItems;
        }
    }
}