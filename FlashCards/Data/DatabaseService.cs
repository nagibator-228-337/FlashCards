using FlashCards.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.Data
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory; //folder with exe
            _dbPath = Path.Combine(folder, "flashcards.db");
        }

        public void InitDataBase()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Cards (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Word TEXT NOT NULL,
                    Translation TEXT NOT NULL,
                    Sentence TEXT,
                    ImagePath TEXT,
                    KnownLevel INTEGER DEFAULT 0,
                    LastReviewed TEXT,
                    NextAvailableAfter INTEGER DEFAULT 0,
                    CreatedAt TEXT,
                    UpdatedAt TEXT
                );
            ";

            using var command = connection.CreateCommand();
            command.CommandText = createTableQuery;
            command.ExecuteNonQuery();
        }

        public void AddCard(Card card)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            string query = @"
            INSERT INTO Cards 
            (Word, Translation, Sentence, ImagePath, KnownLevel, LastReviewed, NextAvailableAfter, CreatedAt, UpdatedAt)
            VALUES 
            (@word, @translation, @sentence, @image, @level, @last, @next, @created, @updated);
            ";

            using var command = connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@word", card.Word);
            command.Parameters.AddWithValue("@translation", card.Translation);
            command.Parameters.AddWithValue("@sentence", card.Sentence ?? "");
            command.Parameters.AddWithValue("@image", card.ImagePath ?? "");
            command.Parameters.AddWithValue("@level", card.KnownLevel);
            command.Parameters.AddWithValue("@last", card.LastReviewed?.ToString("o") ?? (object)DBNull.Value); 
            command.Parameters.AddWithValue("@next", card.NextAvailableAfter);
            command.Parameters.AddWithValue("@created", card.CreatedAt.ToString("o"));
            command.Parameters.AddWithValue("@updated", card.UpdatedAt.ToString("o"));
            command.ExecuteNonQuery();
        }

        public List<Card> CardReader()
        {
            List<Card> cards = new List<Card>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Cards";
            using var reader = command.ExecuteReader();

            while (reader.Read()) 
            {
                string word = (string)reader["Word"];
                string translation = reader["Translation"] is DBNull ? "" : (string)reader["Translation"];
                int level = Convert.ToInt32(reader["KnownLevel"]);
                Card card = new Card()
                {
                    Word = word,
                    Translation = translation,
                    KnownLevel = level
                };
                cards.Add(card);
            }
            return cards;
        }

    }
}
