using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.Models
{
    public class Card
    {
        public int Id { get; set; }                 // автогенерируемый ключ в SQLite
        public string? Word { get; set; }           // основное слово
        public string? Translation { get; set; }   
        public string? Sentence { get; set; }       // пример / предложение 
        public string? ImagePath { get; set; }      
        public int KnownLevel { get; set; }        
        public DateTime? LastReviewed { get; set; }
        public int NextAvailableAfter { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Card()
        {
            KnownLevel = 0;
            NextAvailableAfter = 0;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
