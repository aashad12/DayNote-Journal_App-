using DayNote.Models;
using SQLite;

namespace DayNote.Services
{
    // Service responsible for saving a new journal entry for the current day
    public class EntryService
    {
        private readonly SQLiteAsyncConnection connection;

        // Inject database connection
        public EntryService(JournalDatabase database)
        {
            connection = database.Connection;
        }

        // Saves today's journal entry and enforces one-entry-per-day rule
        public async Task<string> SaveEntryAsync(
            string title,
            string content,
            string primaryMood,
            string? secondaryMood1,
            string? secondaryMood2,
            string? tags)
        {
            // Validate required primary mood
            if (string.IsNullOrWhiteSpace(primaryMood))
                return "Please select a primary mood!";

            var today = DateTime.Today;

            // Check if an entry already exists for today
            var exists = await connection.Table<JournalEntry>()
                .Where(e => e.EntryDate >= today && e.EntryDate < today.AddDays(1))
                .FirstOrDefaultAsync();

            if (exists != null)
                return "An entry for today already exists.";

            // Insert new journal entry with system-generated timestamps
            await connection.InsertAsync(new JournalEntry
            {
                EntryDate = today,
                Title = title,
                Content = content,
                PrimaryMood = primaryMood,
                SecondaryMood1 = secondaryMood1,
                SecondaryMood2 = secondaryMood2,
                Tags = tags,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            return "Journal entry saved successfully!";
        }
    }
}
