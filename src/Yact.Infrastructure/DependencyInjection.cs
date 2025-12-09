using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yact.Application.Interfaces;
using Yact.Domain.Repositories;
using Yact.Infrastructure.FileStorage;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Repositories;

namespace Yact.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DB context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<ICyclistRepository, CyclistRepository>();

        // Storage
        services.Configure<FileStorageConfiguration>(
            configuration.GetSection(FileStorageConfiguration.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IActivityReaderService, ActivityReaderService>();

        return services;
    }
}
