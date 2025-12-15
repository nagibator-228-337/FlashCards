using FlashCards.Data;
using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashCards
{
    internal class LearningSessionManager
    {
        private Queue<SessionCard>? _queue;
        private SessionCard? _current;
        private const int PoolSize = 15;

        public void StartSession()
        {
            var db = new DatabaseService();
            List<Card> listOfCards = db.CardReader();

            var pool = GetPoolOfCards(listOfCards)
                .OrderBy(x => Guid.NewGuid())
                .Select(card => new SessionCard(card))
                .ToList();

            _queue = new Queue<SessionCard>(pool);
            _current = null;
        }

        public bool HasNext()
        {
            if (_queue == null) return false;
            // any non-learned left in queue
            return _queue.Any(c => !c.IsLearned);
        }

        // Dequeues next non-learned card and sets it as current
        public SessionCard? Next()
        {
            if (_queue == null) return null;

            while (_queue.Count > 0)
            {
                var card = _queue.Dequeue();
                if (card.IsLearned)
                {
                    // skip learned
                    continue;
                }
                _current = card;
                return card;
            }

            // no more
            _current = null;
            return null;
        }

        // User swiped left: increase left swipes, slightly decrease known level and re-enqueue
        public void SwipeLeft()
        {
            if (_current == null || _current.Card == null || _queue == null) return;

            _current.LeftSwipes++;
            _current.Card.KnownLevel = Math.Max(0, _current.Card.KnownLevel - 1);

            // re-enqueue for future repetition
            _queue.Enqueue(_current);

            // persist change to DB for KnownLevel
            var db = new DatabaseService();
            _current.Card.UpdatedAt = DateTime.UtcNow;
            db.UpdateCard(_current.Card);

            _current = null;
        }

        // User swiped right: mark learned, increase known level and persist, do not re-enqueue
        public void SwipeRight()
        {
            if (_current == null || _current.Card == null) return;

            _current.IsLearned = true;
            _current.Card.KnownLevel++;
            _current.Card.UpdatedAt = DateTime.UtcNow;

            var db = new DatabaseService();
            db.UpdateCard(_current.Card);

            _current = null;
        }

        public void FlipCurrent()
        {
            if (_current == null) return;
            _current.IsFlipped = !_current.IsFlipped;
        }

        private Card GetRandomCard(List<Card> cards)
        {
            var rnd = new Random();

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

            while (result.Count < PoolSize && pool.Count > 0)
            {
                var card = GetRandomCard(pool);
                result.Add(card);
                pool.Remove(card);
            }

            return result;
        }
    }
}
