using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureVault.Domain.Interfaces;
using SecureVault.Infrastructure.Data;
using SecureVault.Infrastructure.Repositories;
using SecureVault.Infrastructure.Services;

namespace SecureVault.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                }));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories (if not using UoW pattern exclusively)
        services.AddScoped<INotesRepository, NotesRepository>();

        // Infrastructure Services
        services.AddSingleton<IEncryptionService, EncryptionService>();

        return services;
    }
}