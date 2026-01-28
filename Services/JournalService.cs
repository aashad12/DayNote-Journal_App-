using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using DayNote.Models;

namespace DayNote.Services
{
    // Service responsible for all journal entry operations (CRUD, search, pagination)
    public class JournalService
    {
        private readonly SQLiteAsyncConnection connection;
        // Inject database connection
        public JournalService(JournalDatabase database)
        {
            connection = database.Connection;
        }

        // Returns all journal entries
        public Task<List<JournalEntry>> GetAllAsync()
        {
            return connection.Table<JournalEntry>().ToListAsync();
        }

        // Returns a journal entry by its ID
        public Task<JournalEntry> GetByIdAsync(int id)
        {
            return connection.Table<JournalEntry>()
                             .Where(e => e.Id == id)
                             .FirstOrDefaultAsync();
        }

        // Adds a new journal entry (enforces one entry per day rule)
        public async Task AddAsync(JournalEntry entry)
        {
          
            entry.EntryDay = entry.EntryDate.ToString("yyyy-MM-dd");

            
            if (await HasEntryForDayAsync(entry.EntryDate))
                throw new Exception("An entry for this day already exists.");

            await connection.InsertAsync(entry);
        }
        // Returns paginated journal entries for timeline view
        public async Task<List<JournalEntry>> GetEntriesPageAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;

            int offset = (page - 1) * pageSize;

            return await connection.QueryAsync<JournalEntry>(
                "SELECT * FROM JournalEntry ORDER BY EntryDate DESC LIMIT ? OFFSET ?",
                pageSize,
                offset
            );
        }

        // Returns total number of journal entries
        public Task<int> GetTotalCountAsync()
        {
            return connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM JournalEntry"
            );
        }

        // Updates an existing journal entry (prevents duplicate entry per day)
        public async Task UpdateAsync(JournalEntry entry)
        {
            entry.EntryDay = entry.EntryDate.ToString("yyyy-MM-dd");

            // block duplicate (excluding current entry)
            if (await HasEntryForDayAsync(entry.EntryDate, entry.Id))
                throw new Exception("Another entry already exists for this day.");

            await connection.UpdateAsync(entry);
        }

        // Deletes a journal entry
        public Task DeleteAsync(JournalEntry entry)
        {
            return connection.DeleteAsync(entry);
        }

        // Searches journal entries by content/title and mood
        public async Task<List<JournalEntry>> SearchAsync(string? searchText, string? mood)
        {
            searchText = (searchText ?? "").Trim();
            mood = (mood ?? "").Trim();

          
            var sql = new StringBuilder("SELECT * FROM JournalEntry WHERE 1=1");
            var args = new List<object>();

            
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                sql.Append(" AND (Title LIKE ? COLLATE NOCASE OR Content LIKE ? COLLATE NOCASE)");
                var like = $"%{searchText}%";
                args.Add(like);
                args.Add(like);
            }

            
            if (!string.IsNullOrWhiteSpace(mood))
            {
                sql.Append(" AND (PrimaryMood = ? COLLATE NOCASE OR SecondaryMood1 = ? COLLATE NOCASE OR SecondaryMood2 = ? COLLATE NOCASE)");
                args.Add(mood);
                args.Add(mood);
                args.Add(mood);
            }

            
            sql.Append(" ORDER BY EntryDate DESC");

            return await connection.QueryAsync<JournalEntry>(sql.ToString(), args.ToArray());
        }



        // Checks whether a journal entry already exists for a given day
        public async Task<bool> HasEntryForDayAsync(DateTime day, int? excludeId = null)
        {
            var key = day.ToString("yyyy-MM-dd");

            var q = connection.Table<JournalEntry>()
                              .Where(e => e.EntryDay == key);

            if (excludeId.HasValue)
                q = q.Where(e => e.Id != excludeId.Value);

            var exists = await q.FirstOrDefaultAsync();
            return exists != null;
        }

        // Returns total number of journal entries
        public Task<int> GetEntryCountAsync()
        {
            return connection.Table<JournalEntry>().CountAsync();
        }

        // Returns all distinct dates that contain journal entries (calendar view)
        public async Task<List<DateTime>> GetDistinctEntryDatesAsync()
        {
            var entries = await connection.Table<JournalEntry>().ToListAsync();

            return entries
                .Select(e => e.EntryDate.Date)
                .Distinct()
                .ToList();
        }

        // Returns count of distinct moods used across all entries
        public Task<int> GetDistinctMoodCountAsync() =>
              connection.ExecuteScalarAsync<int>(
                  @"SELECT COUNT(DISTINCT Mood)
              FROM (
                  SELECT PrimaryMood AS Mood FROM JournalEntry WHERE PrimaryMood IS NOT NULL
                  UNION
                  SELECT SecondaryMood1 FROM JournalEntry WHERE SecondaryMood1 IS NOT NULL
                  UNION
                  SELECT SecondaryMood2 FROM JournalEntry WHERE SecondaryMood2 IS NOT NULL
              )");

        // Returns the most recent journal entry date
        public Task<DateTime?> GetLatestEntryDateAsync()
        {
            return connection.ExecuteScalarAsync<DateTime?>(
                "SELECT MAX(EntryDate) FROM JournalEntry");
        }

        // Returns the oldest journal entry date

        public Task<DateTime?> GetOldestEntryDateAsync()
        {
            return connection.ExecuteScalarAsync<DateTime?>(
                "SELECT MIN(EntryDate) FROM JournalEntry");
        }

        // Returns journal entries within a selected date range
        public Task<List<JournalEntry>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            var toEnd = to.Date.AddDays(1).AddTicks(-1);

            return connection.Table<JournalEntry>()
                .Where(e => e.EntryDate >= from.Date && e.EntryDate <= toEnd)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();
        }

    }
}
