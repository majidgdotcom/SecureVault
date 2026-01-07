using SecureVault.Domain.Entities;

namespace SecureVault.Domain.Interfaces;

public interface INotesRepository
{
    Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Note>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Note> AddAsync(Note note, CancellationToken cancellationToken = default);
    Task<Note> UpdateAsync(Note note, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}