using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    [CreateAssetMenu(fileName = "New Card Collection", menuName = "RNGNeeds/Card Decks/Card Collection")]
    public class CardCollection : ScriptableObject, IProbabilityItemColorProvider
    {
        public CardRarity rarity;
        public ProbabilityList<Card> cards;
        public Color ItemColor => rarity ? rarity.ItemColor : Color.magenta;
    }
}