using System.Security.Cryptography;

namespace DayNote.Security;

// Provides secure hashing and verification for PIN-based authentication
public static class PinCrypto
{
    // Hashes the PIN using PBKDF2 with a random salt
    public static (string hashB64, string saltB64) HashPin(string pin)
    {
        // Generate a cryptographically secure random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Derive a secure hash from the PIN using PBKDF2
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: pin,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        // Return hash and salt encoded in Base64 for storage
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    // Verifies a PIN by recomputing and comparing hashes securely
    public static bool VerifyPin(string pin, string storedHashB64, string storedSaltB64)
    {
        byte[] salt = Convert.FromBase64String(storedSaltB64);

        // Recreate hash using stored salt and same parameters 

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: pin,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256);

        byte[] computed = pbkdf2.GetBytes(32);
        byte[] stored = Convert.FromBase64String(storedHashB64);

        // Constant-time comparison to prevent timing attacks 
        return CryptographicOperations.FixedTimeEquals(computed, stored);
    }
}
