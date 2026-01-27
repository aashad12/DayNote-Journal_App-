using DayNote.Components.Models;
using SQLite;

namespace DayNote.Components.Services
{
    public class EntryService
    {
        private readonly SQLiteAsyncConnection connection;

        public EntryService(JournalDatabase database)
        {
            connection = database.Connection;
        }

        public async Task<string> SaveEntryAsync(
            string title,
            string content,
            string primaryMood,
            string? secondaryMood1,
            string? secondaryMood2,
            string? tags)
        {
            if (string.IsNullOrWhiteSpace(primaryMood))
                return "Please select a primary mood!";

            var today = DateTime.Today;

            var exists = await connection.Table<JournalEntry>()
                .Where(e => e.EntryDate >= today && e.EntryDate < today.AddDays(1))
                .FirstOrDefaultAsync();

            if (exists != null)
                return "An entry for today already exists.";

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
