using System.Security.Cryptography;

namespace Movies.Api.Utils;

// Class provided by ChatGPT
public class PasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32; // 256-bit
    private const int Iterations = 10000; // Number of iterations for PBKDF2

    public static string HashPassword(string password)
    {
        // Generate a new, random salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Derive the key (hash)
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(KeySize);

        // Combine the salt and hash
        var hashBytes = new byte[SaltSize + KeySize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

        // Convert to Base64 for easier storage
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);

        // Extract the salt from the stored hash
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Hash the input password using the extracted salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(KeySize);

        // Compare the hash of the input password to the stored hash
        for (var i = 0; i < KeySize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
                return false; // Password does not match
        }

        return true; // Password matches
    }
}