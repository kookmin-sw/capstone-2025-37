using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    [CreateAssetMenu(fileName = "New Deck Builder", menuName = "RNGNeeds/Card Decks/Deck Builder")]
    public class DeckBuilder : ScriptableObject
    {
        public PLCollection<CardCollection> cardCollections;
        public bool clearDeckBeforeFill;
        public int maxCards;
        public CardDeck deckToFill;

        public void FillDeck()
        {
            if (clearDeckBeforeFill) deckToFill.cards.ClearList();
            
            var pickedCollections = cardCollections.PickFromAll();
            var pickedCards = new List<Card>();
            
            foreach (var pickedCollection in pickedCollections)
            {
                pickedCards.AddRange(pickedCollection.cards.PickValues());
            }
            
            while (pickedCards.Count > maxCards)
            {
                pickedCards.RemoveAt(pickedCards.Count - 1);
            }
            
            foreach (var pickedCard in pickedCards)
            {
                deckToFill.cards.AddItem(new ProbabilityItem<Card>(pickedCard, 1f));
            }
            
            deckToFill.cards.NormalizeProbabilities();
        }
    }
}