using DayNote.Models;

namespace DayNote.Services
{
    public partial class FeatureService
    {

        private readonly JournalService _journalService;

        public FeatureService(JournalService journalService)
        {
            _journalService = journalService;
        }
        // Mood Distribution (by primary mood)
        public Dictionary<string, int> CalculateMoodDistribution(List<JournalEntry> entries)
        {
            //  Adjust "PrimaryMood" property name 
            return entries
                .Where(e => !string.IsNullOrWhiteSpace(e.PrimaryMood))
                .GroupBy(e => e.PrimaryMood.Trim())
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Tag counts (top tags)
        public Dictionary<string, int> CalculateTagCounts(List<JournalEntry> entries)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                //  Adjust "Tags" property 
                if (string.IsNullOrWhiteSpace(entry.Tags)) continue;

                var tags = entry.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var raw in tags)
                {
                    var t = raw.Trim();
                    if (string.IsNullOrWhiteSpace(t)) continue;

                    if (!dict.ContainsKey(t)) dict[t] = 0;
                    dict[t]++;
                }
            }

            return dict;
        }

        // Tag breakdown % 
        public Dictionary<string, double> CalculateTagBreakdownPercent(List<JournalEntry> entries)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            if (entries.Count == 0) return result;

            // For each tag, count how many entries contain it at least once
            var tagEntryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Tags)) continue;

                var uniqueTags = entry.Tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var tag in uniqueTags)
                {
                    if (!tagEntryCounts.ContainsKey(tag)) tagEntryCounts[tag] = 0;
                    tagEntryCounts[tag]++;
                }
            }

            foreach (var kv in tagEntryCounts)
            {
                var percent = kv.Value / (double)entries.Count * 100;
                result[kv.Key] = percent;
            }

            return result;
        }

        
        public List<WordTrendPoint> CalculateWordTrend(List<JournalEntry> entries)
        {
           
            return entries
                .GroupBy(e => e.EntryDay)
                .Select(g => new WordTrendPoint
                {
                    Day = g.Key,
                    WordCount = g.Sum(e => CountWords(e.Content))
                })
                .OrderBy(x => DateTime.Parse(x.Day))
                .ToList();
        }

        private int CountWords(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        public async Task<int> CalculateCurrentStreakAsync()
        {
            var entries = await _journalService.GetAllAsync();

            var dates = entries
                .Select(e => DateTime.Parse(e.EntryDay).Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (!dates.Any())
                return 0;

            var today = DateTime.Today;

            // If latest entry is older than yesterday → streak is 0
            if ((today - dates[0]).Days > 1)
                return 0;

            int streak = 1; // at least 1 day streak
            var expectedDate = dates[0].AddDays(-1);

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i] == expectedDate)
                {
                    streak++;
                    expectedDate = expectedDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }


        public async Task<int> CalculateLongestStreakAsync()
        {
            var entries = await _journalService.GetAllAsync();
            var dates = entries
                .Select(e => DateTime.Parse(e.EntryDay))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            int longest = 0;
            int current = 0;

            for (int i = 0; i < dates.Count; i++)
            {
                if (i == 0 || dates[i] == dates[i - 1].AddDays(1))
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else
                {
                    current = 1;
                }
            }

            return longest;
        }

        // Missed dates list (for last 30 days)
        // This uses your existing DB logic; if you already have something similar, keep one.
        public async Task<int> GetMissedDaysLast30Async()
        {
            var entries = await _journalService.GetAllAsync();
            var entryDates = entries
                .Select(e => DateTime.Parse(e.EntryDay))
                .ToHashSet();

            int missed = 0;

            for (int i = 0; i < 30; i++)
            {
                var day = DateTime.Today.AddDays(-i);
                if (!entryDates.Contains(day))
                    missed++;
            }

            return missed;
        }
        public async Task<List<DateTime>> GetMissedDatesLast30Async()
        {
            var entries = await _journalService.GetAllAsync();
            var entryDates = entries
            .Select(e => DateTime.Parse(e.EntryDay).Date)
            .ToHashSet();
            var missed = new List<DateTime>();

            for (int i = 0; i < 30; i++)
            {
                var day = DateTime.Today.AddDays(-i).Date;
                if (!entryDates.Contains(day))
                    missed.Add(day);
            }

            return missed;
        }

    }
}
