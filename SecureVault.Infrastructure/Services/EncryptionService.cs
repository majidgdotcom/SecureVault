using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SecureVault.Domain.Interfaces;

namespace SecureVault.Infrastructure.Services;

/// <summary>
/// Encryption service implementation - Infrastructure concern
/// Implements interface defined in Domain (Dependency Inversion)
/// Uses AES-256-CBC with IV prepended to ciphertext for secure storage
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private const int IvSize = 16; // 128 bits for AES

    public EncryptionService(IConfiguration configuration)
    {
        var masterKey = configuration["Encryption:MasterKey"] 
            ?? throw new InvalidOperationException(
                "Encryption master key not configured. " +
                "Set Encryption:MasterKey in configuration.");
        
        // Derive 256-bit key using PBKDF2
        using var deriveBytes = new Rfc2898DeriveBytes(
            masterKey, 
            Encoding.UTF8.GetBytes("SecureVault.Salt.v1"), 
            iterations: 100000, 
            HashAlgorithmName.SHA256
        );
        
        _key = deriveBytes.GetBytes(32); // 256 bits = 32 bytes
    }

    /// <summary>
    /// Encrypts plaintext and returns Base64 string with IV prepended to ciphertext.
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV(); // Generate unique IV for each encryption

        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        var ciphertextBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // Prepend IV to ciphertext
        var result = new byte[IvSize + ciphertextBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, IvSize);
        Buffer.BlockCopy(ciphertextBytes, 0, result, IvSize, ciphertextBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts Base64 string by extracting IV and decrypting ciphertext.
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));

        byte[] fullCipher;
        try
        {
            fullCipher = Convert.FromBase64String(encryptedText);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 format.", nameof(encryptedText), ex);
        }

        if (fullCipher.Length < IvSize)
            throw new ArgumentException("Encrypted text is too short to contain IV.", nameof(encryptedText));

        // Extract IV and ciphertext
        var iv = new byte[IvSize];
        var ciphertext = new byte[fullCipher.Length - IvSize];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, IvSize);
        Buffer.BlockCopy(fullCipher, IvSize, ciphertext, 0, ciphertext.Length);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plaintextBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}