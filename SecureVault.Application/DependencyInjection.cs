using Microsoft.Extensions.DependencyInjection;
using SecureVault.Application.Interfaces;
using SecureVault.Application.Services;

namespace SecureVault.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INotesService, NotesService>();

        return services;
    }
}