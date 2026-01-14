using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yact.Application.Interfaces.Files;
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
        // DB contexts
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<ReadDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(WriteDbContext).Assembly));

        // Repositories
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<ICyclistRepository, CyclistRepository>();
        services.AddScoped<IClimbRepository, ClimbRepository>();
        services.AddScoped<IActivityClimbRepository, ActivityClimbRepository>();

        // Storage
        services.Configure<FileStorageConfiguration>(
            configuration.GetSection(FileStorageConfiguration.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IActivityReaderService, ActivityReaderService>();

        return services;
    }
}
