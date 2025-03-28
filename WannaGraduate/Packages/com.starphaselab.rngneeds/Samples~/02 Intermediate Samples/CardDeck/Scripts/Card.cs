using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    [CreateAssetMenu(fileName = "New Card", menuName = "RNGNeeds/Card Decks/Card")]
    public class Card : ScriptableObject, IProbabilityItemColorProvider
    {
        public CardRarity cardRarity;
        public Color ItemColor => cardRarity == null ? Color.magenta : cardRarity.rarityColor;
    }
}