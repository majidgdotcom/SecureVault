using SecureVault.Application.DTOs;

namespace SecureVault.Application.Interfaces;

public interface INotesService
{
    Task<Result<NoteResponseDto>> CreateNoteAsync(
        CreateNoteDto dto, 
        CancellationToken cancellationToken = default);

    Task<Result<NoteResponseDto>> GetNoteAsync(
        int id, 
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<NoteResponseDto>>> GetUserNotesAsync(
        string userId, 
        CancellationToken cancellationToken = default);

    Task<Result<NoteResponseDto>> UpdateNoteAsync(
        int id, 
        UpdateNoteDto dto, 
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteNoteAsync(
        int id, 
        CancellationToken cancellationToken = default);
}