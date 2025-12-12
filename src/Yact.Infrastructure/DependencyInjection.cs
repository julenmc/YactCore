using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yact.Application.Interfaces;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.ClimbFinder;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Smoothers.Altitude;
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
        services.AddScoped<IClimbRepository, ClimbRepository>();
        services.AddScoped<IActivityClimbRepository, ActivityClimbRepository>();

        // Storage
        services.Configure<FileStorageConfiguration>(
            configuration.GetSection(FileStorageConfiguration.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IActivityReaderService, ActivityReaderService>();

        // Domain Services
        services.AddScoped<IDistanceCalculator, HarversineDistanceCalculatorService>();
        services.AddScoped<IAltitudeSmootherService, WeightedDistanceAltitudeSmoother>();
        services.AddScoped<IClimbFinderService, ClimbFinderService>();
        services.AddScoped<IRouteAnalyzerService, RouteAnalyzerService>();

        return services;
    }
}
