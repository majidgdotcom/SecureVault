using Microsoft.Extensions.Logging;
using SecureVault.Application.DTOs;
using SecureVault.Application.Interfaces;
using SecureVault.Domain.Entities;
using SecureVault.Domain.Interfaces;

namespace SecureVault.Application.Services;

public class NotesService : INotesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<NotesService> _logger;

    public NotesService(
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService,
        ILogger<NotesService> logger)
    {
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task<Result<NoteResponseDto>> CreateNoteAsync(
        CreateNoteDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating note for user {UserId}", dto.UserId);

            // Encrypt the content
            var encryptedContent = _encryptionService.Encrypt(dto.Content);

            // Create domain entity using factory method
            var note = Note.Create(encryptedContent, dto.UserId);

            // Persist through repository
            var createdNote = await _unitOfWork.Notes.AddAsync(note, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Note {NoteId} created successfully", createdNote.Id);

            // Decrypt for response
            var decryptedContent = _encryptionService.Decrypt(
                createdNote.EncryptedContent);

            return Result<NoteResponseDto>.Success(new NoteResponseDto
            {
                Id = createdNote.Id,
                Content = decryptedContent,
                CreatedAt = createdNote.CreatedAt,
                UpdatedAt = createdNote.UpdatedAt,
                UserId = createdNote.UserId
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating note");
            return Result<NoteResponseDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note for user {UserId}", dto.UserId);
            return Result<NoteResponseDto>.Failure("An error occurred while creating the note");
        }
    }

    public async Task<Result<NoteResponseDto>> GetNoteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id, cancellationToken);

            if (note == null)
            {
                return Result<NoteResponseDto>.Failure($"Note with ID {id} not found");
            }

            // Decrypt content
            var decryptedContent = _encryptionService.Decrypt(
                note.EncryptedContent);

            return Result<NoteResponseDto>.Success(new NoteResponseDto
            {
                Id = note.Id,
                Content = decryptedContent,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                UserId = note.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {NoteId}", id);
            return Result<NoteResponseDto>.Failure("An error occurred while retrieving the note");
        }
    }

    public async Task<Result<IEnumerable<NoteResponseDto>>> GetUserNotesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notes = await _unitOfWork.Notes.GetByUserIdAsync(userId, cancellationToken);

            var noteDtos = notes.Select(note =>
            {
                try
                {
                    var decryptedContent = _encryptionService.Decrypt(
                        note.EncryptedContent);

                    return new NoteResponseDto
                    {
                        Id = note.Id,
                        Content = decryptedContent,
                        CreatedAt = note.CreatedAt,
                        UpdatedAt = note.UpdatedAt,
                        UserId = note.UserId
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error decrypting note {NoteId}", note.Id);
                    return null;
                }
            })
            .Where(n => n != null)
            .Cast<NoteResponseDto>()
            .ToList();

            return Result<IEnumerable<NoteResponseDto>>.Success(noteDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes for user {UserId}", userId);
            return Result<IEnumerable<NoteResponseDto>>.Failure(
                "An error occurred while retrieving notes");
        }
    }

    public async Task<Result<NoteResponseDto>> UpdateNoteAsync(
        int id,
        UpdateNoteDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id, cancellationToken);

            if (note == null)
            {
                return Result<NoteResponseDto>.Failure($"Note with ID {id} not found");
            }

            // Encrypt new content
            var encryptedContent = _encryptionService.Encrypt(dto.Content);

            // Update using domain method
            note.UpdateContent(encryptedContent);

            // Persist changes
            await _unitOfWork.Notes.UpdateAsync(note, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Note {NoteId} updated successfully", id);

            return Result<NoteResponseDto>.Success(new NoteResponseDto
            {
                Id = note.Id,
                Content = dto.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                UserId = note.UserId
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating note {NoteId}", id);
            return Result<NoteResponseDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId}", id);
            return Result<NoteResponseDto>.Failure("An error occurred while updating the note");
        }
    }

    public async Task<Result<bool>> DeleteNoteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _unitOfWork.Notes.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                return Result<bool>.Failure($"Note with ID {id} not found");
            }

            var deleted = await _unitOfWork.Notes.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Note {NoteId} deleted successfully", id);

            return Result<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            return Result<bool>.Failure("An error occurred while deleting the note");
        }
    }
}