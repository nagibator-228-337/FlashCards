using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards
{
    internal class SessionCard
    {
        public Card? Card { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsLearned { get; set; }
        public int LeftSwipes { get; set; }
        

        public SessionCard(Card card)
        {
            Card = card;
            IsFlipped = false;
            IsLearned = false;
            LeftSwipes = 0;
        }



    }
}
