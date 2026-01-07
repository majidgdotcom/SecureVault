using Microsoft.EntityFrameworkCore;
using SecureVault.Domain.Entities;

namespace SecureVault.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Note>(entity =>
        {
            entity.ToTable("Notes");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EncryptedContent)
                .IsRequired()
                .HasMaxLength(4000);

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired(false);

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Notes_UserId");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Notes_CreatedAt");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                .HasDatabaseName("IX_Notes_UserId_CreatedAt");
        });
    }
}