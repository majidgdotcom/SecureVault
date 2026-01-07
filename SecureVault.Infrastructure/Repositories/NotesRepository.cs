using Microsoft.EntityFrameworkCore;
using SecureVault.Domain.Entities;
using SecureVault.Domain.Interfaces;
using SecureVault.Infrastructure.Data;

namespace SecureVault.Infrastructure.Repositories;

public class NotesRepository : INotesRepository
{
    private readonly AppDbContext _context;

    public NotesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Note?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Note>> GetByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Note> AddAsync(Note note, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Notes.AddAsync(note, cancellationToken);
        return entry.Entity;
    }

    public async Task<Note> UpdateAsync(Note note, CancellationToken cancellationToken = default)
    {
        _context.Notes.Update(note);
        return await Task.FromResult(note);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var note = await _context.Notes.FindAsync(new object[] { id }, cancellationToken);
        
        if (note == null)
            return false;

        _context.Notes.Remove(note);
        return true;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Notes
            .AnyAsync(n => n.Id == id, cancellationToken);
    }
}