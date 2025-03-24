using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest
{
    public abstract class ChestBase : ScriptableObject
    {
        public int playerLevel;
        [HideInInspector] public List<Item> chestContents;
        public abstract List<Item> OpenChest();
    }
}