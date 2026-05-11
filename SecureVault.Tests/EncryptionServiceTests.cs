using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SecureVault.Infrastructure.Services;

namespace SecureVault.Tests;

public class EncryptionServiceTests
{
    private readonly EncryptionService _sut;

    public EncryptionServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:MasterKey"] = "test-master-key-that-is-long-enough-32chars!"
            })
            .Build();

        _sut = new EncryptionService(config);
    }

    [Fact]
    public void Encrypt_ShouldReturnBase64String()
    {
        var result = _sut.Encrypt("hello world");

        result.Should().NotBeNullOrEmpty();
        var bytes = Convert.FromBase64String(result); // should not throw
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void Decrypt_ShouldReturnOriginalPlaintext()
    {
        var plaintext = "my secret note content";

        var encrypted = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted);

        decrypted.Should().Be(plaintext);
    }

    [Fact]
    public void Encrypt_SamePlaintext_ShouldProduceDifferentCiphertexts()
    {
        // Each encryption uses a unique IV, so results must differ
        var plaintext = "same content";

        var first = _sut.Encrypt(plaintext);
        var second = _sut.Encrypt(plaintext);

        first.Should().NotBe(second);
    }

    [Fact]
    public void Encrypt_EmptyString_ShouldThrowArgumentException()
    {
        var act = () => _sut.Encrypt(string.Empty);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Plain text cannot be null or empty*");
    }

    [Fact]
    public void Decrypt_InvalidBase64_ShouldThrowArgumentException()
    {
        var act = () => _sut.Decrypt("not-valid-base64!!!");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid Base64*");
    }

    [Fact]
    public void Decrypt_TooShortInput_ShouldThrowArgumentException()
    {
        // Valid Base64 but shorter than IV size (16 bytes)
        var tooShort = Convert.ToBase64String(new byte[8]);
        var act = () => _sut.Decrypt(tooShort);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*too short*");
    }

    [Theory]
    [InlineData("simple text")]
    [InlineData("text with special chars: !@#$%^&*()")]
    [InlineData("unicode: 日本語テスト")]
    [InlineData("multiline\nnote\ncontent")]
    public void RoundTrip_VariousInputs_ShouldDecryptCorrectly(string plaintext)
    {
        var encrypted = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted);

        decrypted.Should().Be(plaintext);
    }
}
