using SQLite;
using DayNote.Components.Models;

namespace DayNote.Components.Services
{
    public class JournalDatabase
    {
        public SQLiteAsyncConnection Connection { get; private set; }
        public JournalDatabase()
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal.db");
            Connection = new SQLiteAsyncConnection(dbPath);

            
            Connection.CreateTableAsync<JournalEntry>().Wait();
            Connection.CreateTableAsync<PinSetting>().Wait();
        }

      
        
    }
}
