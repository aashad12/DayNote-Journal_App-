using DayNote.Components.Models;
using DayNote.Components.Security;
using SQLite;

namespace DayNote.Components.Services
{
    public class PinRepository
    {
        private readonly JournalDatabase _db;

        public PinRepository(JournalDatabase db)
        {
            _db = db;
        }

        private SQLiteAsyncConnection Conn => _db.Connection;

        public async Task<bool> HasPinAsync()
        {
            var row = await Conn.Table<PinSetting>()
                                .Where(x => x.Id == 1)
                                .FirstOrDefaultAsync();

            return row != null &&
                   !string.IsNullOrWhiteSpace(row.PinHash) &&
                   !string.IsNullOrWhiteSpace(row.PinSalt);
        }

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

        public async Task<bool> VerifyAsync(string pin)
        {
            var row = await Conn.Table<PinSetting>()
                                .Where(x => x.Id == 1)
                                .FirstOrDefaultAsync();

            if (row == null) return false;

            return PinCrypto.VerifyPin(pin, row.PinHash, row.PinSalt);
        }

        public async Task DisableAsync()
        {
            await Conn.DeleteAsync<PinSetting>(1);
        }
    }
}
