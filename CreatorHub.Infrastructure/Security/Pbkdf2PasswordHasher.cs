using System.Security.Cryptography;
using CreatorHub.Application.Interfaces.Services;

namespace CreatorHub.Infrastructure.Security;

/// <summary>PBKDF2-HMAC-SHA256. Stored as: v1.{iterations}.{saltBase64}.{hashBase64}</summary>
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string storedHash, string password)
    {
        var parts = storedHash.Split('.');

        if (parts.Length != 4 || parts[0] != "v1" ||
            !int.TryParse(parts[1], out var iterations) || iterations < 1 || iterations > 5_000_000)
            return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
