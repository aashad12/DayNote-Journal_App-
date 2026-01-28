using DayNote.Models;
using DayNote.Security;
using SQLite;

namespace DayNote.Services
{
    // Repository responsible for managing PIN-based security
    public class PinRepository
    {
        private readonly JournalDatabase _db;

        // Inject database dependency
        public PinRepository(JournalDatabase db)
        {
            _db = db;
        }
        // Shortcut to SQLite async connection
        private SQLiteAsyncConnection Conn => _db.Connection;

        // Checks whether a PIN is already set
        public async Task<bool> HasPinAsync()
        {
            var row = await Conn.Table<PinSetting>()
                                .Where(x => x.Id == 1)
                                .FirstOrDefaultAsync();

            return row != null &&
                   !string.IsNullOrWhiteSpace(row.PinHash) &&
                   !string.IsNullOrWhiteSpace(row.PinSalt);
        }

        // Sets or updates the PIN (stored securely as hash + salt)
        public async Task SetPinAsync(string pin)
        {
            var (hash, salt) = PinCrypto.HashPin(pin);

            var row = new PinSetting
            {
                Id = 1,
                PinHash = hash,
                PinSalt = salt
            };

            await Conn.InsertOrReplaceAsync(row);
        }
        // Verifies user-entered PIN against stored hash
        public async Task<bool> VerifyAsync(string pin)
        {
            var row = await Conn.Table<PinSetting>()
                                .Where(x => x.Id == 1)
                                .FirstOrDefaultAsync();

            if (row == null) return false;

            return PinCrypto.VerifyPin(pin, row.PinHash, row.PinSalt);
        }

        // Disables PIN protection by removing stored credentials
        public async Task DisableAsync()
        {
            await Conn.DeleteAsync<PinSetting>(1);
        }
    }
}
