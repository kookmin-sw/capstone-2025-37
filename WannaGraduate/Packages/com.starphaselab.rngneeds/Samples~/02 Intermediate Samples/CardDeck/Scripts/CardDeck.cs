using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    [CreateAssetMenu(fileName = "New Card Deck", menuName = "RNGNeeds/Card Decks/Card Deck")]
    public class CardDeck : ScriptableObject
    {
        public ProbabilityList<Card> cards;

        public Card Peek()
        {
            return cards.GetProbabilityItem(0).Value;
        }

        public List<Card> DrawFromTop(int count)
        {
            var pickedCards = new List<Card>();
            var drawCount = Mathf.Clamp(count, count, cards.ItemCount);
            
            for (var i = 1; i <= drawCount; i++)
            {
                var card = cards.GetProbabilityItem(0).Value;
                cards.RemoveItem(card);
                pickedCards.Add(card);
            }

            cards.NormalizeProbabilities();
            return pickedCards;
        }

        public List<Card> DrawFromBottom(int count)
        {
            var pickedCards = new List<Card>();
            var drawCount = Mathf.Clamp(count, count, cards.ItemCount);
            
            for (var i = drawCount; i >= 1; i--)
            {
                var card = cards.GetProbabilityItem(cards.ItemCount - 1).Value;
                cards.RemoveItem(card);
                pickedCards.Add(card);
            }

            cards.NormalizeProbabilities();
            return pickedCards;
        }
        
        public List<Card> DrawRandom(int count)
        {
            var pickedCards = new List<Card>();
            for (var i = 1; i <= count; i++)
            {
                if (cards.TryPickValue(out var pickedCard) == false) break;
                cards.RemoveItem(pickedCard);
                pickedCards.Add(pickedCard);
            }

            cards.NormalizeProbabilities();
            return pickedCards;
        }
    }
        
}