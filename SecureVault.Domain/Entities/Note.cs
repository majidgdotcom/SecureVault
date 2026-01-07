namespace SecureVault.Domain.Entities;

public class Note
{
    public int Id { get; private set; }
    public string EncryptedContent { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string UserId { get; private set; } = string.Empty;

    // Private constructor for EF Core
    private Note() { }

    // Factory method - encapsulates creation logic
    public static Note Create(string encryptedContent, string userId)
    {
        ValidateEncryptedContent(encryptedContent);
        ValidateUserId(userId);

        return new Note
        {
            EncryptedContent = encryptedContent,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Business logic method
    public void UpdateContent(string encryptedContent)
    {
        ValidateEncryptedContent(encryptedContent);
        EncryptedContent = encryptedContent;
        UpdatedAt = DateTime.UtcNow;
    }

    // Domain validation methods
    private static void ValidateEncryptedContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Encrypted content cannot be empty", nameof(content));

        if (content.Length > 4000)
            throw new ArgumentException("Encrypted content exceeds maximum length", nameof(content));
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
    }
}