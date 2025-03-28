using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    [CreateAssetMenu(fileName = "Some Card Rarity", menuName = "RNGNeeds/Card Decks/Card Rarity")]
    public class CardRarity : ScriptableObject, IProbabilityItemInfoProvider, IProbabilityItemColorProvider
    {
        public string rarity;
        public Color rarityColor;
        public string ItemInfo => rarity;
        public Color ItemColor => rarityColor;
    }
}