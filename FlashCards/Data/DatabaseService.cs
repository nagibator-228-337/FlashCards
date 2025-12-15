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

            command.Parameters.AddWithValue("@word", card.Word ?? "");
            command.Parameters.AddWithValue("@translation", card.Translation ?? "");
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
                int id = Convert.ToInt32(reader["Id"]);
                string word = reader["Word"] is DBNull ? "" : (string)reader["Word"];
                string translation = reader["Translation"] is DBNull ? "" : (string)reader["Translation"];
                string sentence = reader["Sentence"] is DBNull ? "" : (string)reader["Sentence"];
                string image = reader["ImagePath"] is DBNull ? "" : (string)reader["ImagePath"];
                int level = reader["KnownLevel"] is DBNull ? 0 : Convert.ToInt32(reader["KnownLevel"]);
                DateTime? lastReviewed = null;
                if (!(reader["LastReviewed"] is DBNull))
                {
                    if (DateTime.TryParse((string)reader["LastReviewed"], out var dt))
                        lastReviewed = dt;
                }
                int nextAvailable = reader["NextAvailableAfter"] is DBNull ? 0 : Convert.ToInt32(reader["NextAvailableAfter"]);
                DateTime created = DateTime.UtcNow;
                DateTime updated = DateTime.UtcNow;
                if (!(reader["CreatedAt"] is DBNull))
                {
                    DateTime.TryParse((string)reader["CreatedAt"], out created);
                }
                if (!(reader["UpdatedAt"] is DBNull))
                {
                    DateTime.TryParse((string)reader["UpdatedAt"], out updated);
                }

                Card card = new Card()
                {
                    Id = id,
                    Word = word,
                    Translation = translation,
                    Sentence = sentence,
                    ImagePath = image,
                    KnownLevel = level,
                    LastReviewed = lastReviewed,
                    NextAvailableAfter = nextAvailable,
                    CreatedAt = created,
                    UpdatedAt = updated
                };
                cards.Add(card);
            }
            return cards;
        }

        public void UpdateCard(Card card)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Cards SET
                    Word = @word,
                    Translation = @translation,
                    Sentence = @sentence,
                    ImagePath = @image,
                    KnownLevel = @level,
                    LastReviewed = @last,
                    NextAvailableAfter = @next,
                    UpdatedAt = @updated
                WHERE Id = @id;
             ";

            command.Parameters.AddWithValue("@id", card.Id);
            command.Parameters.AddWithValue("@word", card.Word ?? "");
            command.Parameters.AddWithValue("@translation", card.Translation ?? "");
            command.Parameters.AddWithValue("@sentence", card.Sentence ?? "");
            command.Parameters.AddWithValue("@image", card.ImagePath ?? "");
            command.Parameters.AddWithValue("@level", card.KnownLevel);
            command.Parameters.AddWithValue("@last", card.LastReviewed?.ToString("o") ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@next", card.NextAvailableAfter);
            command.Parameters.AddWithValue("@updated", card.UpdatedAt.ToString("o"));
            command.ExecuteNonQuery();
        }

    }
}
