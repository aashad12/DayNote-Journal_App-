using System.Security.Cryptography;

namespace DayNote.Components.Security;

public static class PinCrypto
{
    public static (string hashB64, string saltB64) HashPin(string pin)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: pin,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool VerifyPin(string pin, string storedHashB64, string storedSaltB64)
    {
        byte[] salt = Convert.FromBase64String(storedSaltB64);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: pin,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256);

        byte[] computed = pbkdf2.GetBytes(32);
        byte[] stored = Convert.FromBase64String(storedHashB64);

        return CryptographicOperations.FixedTimeEquals(computed, stored);
    }
}
