using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SecureVault.Application.DTOs;
using SecureVault.Application.Services;
using SecureVault.Domain.Entities;
using SecureVault.Domain.Interfaces;

namespace SecureVault.Tests;

public class NotesServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<INotesRepository> _notesRepo;
    private readonly Mock<IEncryptionService> _encryption;
    private readonly NotesService _sut;

    public NotesServiceTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _notesRepo = new Mock<INotesRepository>();
        _encryption = new Mock<IEncryptionService>();

        _unitOfWork.Setup(u => u.Notes).Returns(_notesRepo.Object);

        _sut = new NotesService(
            _unitOfWork.Object,
            _encryption.Object,
            NullLogger<NotesService>.Instance);
    }

    // ── CreateNote ────────────────────────────────────────────────

    [Fact]
    public async Task CreateNoteAsync_ValidInput_ReturnsSuccess()
    {
        var dto = new CreateNoteDto { UserId = "user1", Content = "hello" };
        var fakeNote = Note.Create("encrypted-content", "user1");

        _encryption.Setup(e => e.Encrypt("hello")).Returns("encrypted-content");
        _encryption.Setup(e => e.Decrypt("encrypted-content")).Returns("hello");
        _notesRepo.Setup(r => r.AddAsync(It.IsAny<Note>(), default))
                  .ReturnsAsync(fakeNote);

        var result = await _sut.CreateNoteAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("hello");
        result.Data.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task CreateNoteAsync_EncryptionThrows_ReturnsFailure()
    {
        var dto = new CreateNoteDto { UserId = "user1", Content = "hello" };

        _encryption.Setup(e => e.Encrypt(It.IsAny<string>()))
                   .Throws(new Exception("encryption error"));

        var result = await _sut.CreateNoteAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("error occurred");
    }

    // ── GetNote ───────────────────────────────────────────────────

    [Fact]
    public async Task GetNoteAsync_ExistingNote_ReturnsDecryptedContent()
    {
        var note = Note.Create("encrypted", "user1");
        _notesRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(note);
        _encryption.Setup(e => e.Decrypt("encrypted")).Returns("plaintext");

        var result = await _sut.GetNoteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("plaintext");
    }

    [Fact]
    public async Task GetNoteAsync_NotFound_ReturnsFailure()
    {
        _notesRepo.Setup(r => r.GetByIdAsync(99, default))
                  .ReturnsAsync((Note?)null);

        var result = await _sut.GetNoteAsync(99);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    // ── UpdateNote ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateNoteAsync_ExistingNote_ReturnsUpdatedContent()
    {
        var note = Note.Create("old-encrypted", "user1");
        var dto = new UpdateNoteDto { Content = "new content" };

        _notesRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(note);
        _encryption.Setup(e => e.Encrypt("new content")).Returns("new-encrypted");

        var result = await _sut.UpdateNoteAsync(1, dto);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("new content");
        _notesRepo.Verify(r => r.UpdateAsync(note, default), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateNoteAsync_NotFound_ReturnsFailure()
    {
        _notesRepo.Setup(r => r.GetByIdAsync(99, default))
                  .ReturnsAsync((Note?)null);

        var result = await _sut.UpdateNoteAsync(99, new UpdateNoteDto { Content = "x" });

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    // ── DeleteNote ────────────────────────────────────────────────

    [Fact]
    public async Task DeleteNoteAsync_ExistingNote_ReturnsSuccess()
    {
        _notesRepo.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _notesRepo.Setup(r => r.DeleteAsync(1, default)).ReturnsAsync(true);

        var result = await _sut.DeleteNoteAsync(1);

        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteNoteAsync_NotFound_ReturnsFailure()
    {
        _notesRepo.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.DeleteNoteAsync(99);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _notesRepo.Verify(r => r.DeleteAsync(It.IsAny<int>(), default), Times.Never);
    }

    // ── GetUserNotes ──────────────────────────────────────────────

    [Fact]
    public async Task GetUserNotesAsync_MultpleNotes_ReturnsAllDecrypted()
    {
        var notes = new List<Note>
        {
            Note.Create("enc1", "user1"),
            Note.Create("enc2", "user1"),
        };
        _notesRepo.Setup(r => r.GetByUserIdAsync("user1", default))
                  .ReturnsAsync(notes);
        _encryption.Setup(e => e.Decrypt("enc1")).Returns("note one");
        _encryption.Setup(e => e.Decrypt("enc2")).Returns("note two");

        var result = await _sut.GetUserNotesAsync("user1");

        result.IsSuccess.Should().BeTrue();
        result.Data!.Should().HaveCount(2);
    }
}
