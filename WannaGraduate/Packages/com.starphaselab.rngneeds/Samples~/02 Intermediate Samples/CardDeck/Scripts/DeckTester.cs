using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.CardDeck
{
    public class DeckTester : MonoBehaviour
    {
        public int cardsToDraw = 1;
        public List<Card> hand;
        [Space]
        public CardDeck cardDeck;
        
        [ContextMenu("Peek")]
        public void Peek()
        {
            Debug.Log(cardDeck.Peek().name);
        }

        [ContextMenu("Draw Top")]
        public void DrawFromTop()
        {
            hand.AddRange(cardDeck.DrawFromTop(cardsToDraw));
        }

        [ContextMenu("Draw Bottom")]
        public void DrawFromBottom()
        {
            hand.AddRange(cardDeck.DrawFromBottom(cardsToDraw));
        }
        
        [ContextMenu("Draw Random")]
        public void DrawRandom()
        {
            hand.AddRange(cardDeck.DrawRandom(cardsToDraw));
        }
    }
}