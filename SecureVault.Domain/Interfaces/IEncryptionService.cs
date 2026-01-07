namespace SecureVault.Domain.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText);

    string Decrypt(string encryptedText);
}