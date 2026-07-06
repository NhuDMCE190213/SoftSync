using System.Security.Cryptography;

namespace SoftSync.Common.Security;

/// <summary>
/// Hash & verify mật khẩu bằng PBKDF2 (built-in .NET, không cần cài thêm package).
/// Định dạng lưu: {iterations}.{salt-base64}.{hash-base64}
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;   // 128 bit
    private const int KeySize = 32;    // 256 bit
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out var iterations)) return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        var testKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, key.Length);
        return CryptographicOperations.FixedTimeEquals(key, testKey);
    }
}