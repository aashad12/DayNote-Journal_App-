using SQLite;
using DayNote.Models;

namespace DayNote.Services
{
    // Handles SQLite database initialization and table creation
    public class JournalDatabase
    {
        // Provides a shared asynchronous database connection
        public SQLiteAsyncConnection Connection { get; private set; }
        // Initializes the local SQLite database
        public JournalDatabase()

        {
            // Create database file path in local app storage
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal.db");
            // Initialize SQLite async connection
            Connection = new SQLiteAsyncConnection(dbPath);

            // Create required tables if they do not already exist
            Connection.CreateTableAsync<JournalEntry>().Wait();
            Connection.CreateTableAsync<PinSetting>().Wait();
        }

      
        
    }
}
