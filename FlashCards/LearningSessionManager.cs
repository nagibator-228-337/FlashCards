using FlashCards.Data;
using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FlashCards
{
    internal class LearningSessionManager
    {
        private List<SessionCard>? _sessionCards;
        private int _currentIndex = 0;
        public void StartSession()
        {
            var db = new DatabaseService();
            List<Card> listOfCards = db.CardReader();

            var mixedList = GetPoolOfCards(listOfCards).OrderBy(x => Guid.NewGuid()).ToList();
            _sessionCards = mixedList.Select(card => new SessionCard(card)).ToList();



            
        }

        public void FlipTheCard()
        {

        }

         private Card GetRandomCard (List<Card> cards)
        {
            Random rnd = new Random();

            double totalWeight = 0;

            foreach (var card in cards)
                totalWeight += Math.Exp(-card.KnownLevel);

            double r = rnd.NextDouble() * totalWeight;

            foreach (var card in cards)
            {
                r -= Math.Exp(-card.KnownLevel);
                if (r <= 0) return card;
            }
            return cards[0];
        }

        private List<Card> GetPoolOfCards(List<Card> allCards)
        {
            var result = new List<Card>();
            var pool = new List<Card>(allCards);

            while (result.Count < 15 && pool.Count > 0)
            {
                var card = GetRandomCard(pool);
                result.Add(card);
                pool.Remove(card);
            }

            return result;
        }
    }
}
