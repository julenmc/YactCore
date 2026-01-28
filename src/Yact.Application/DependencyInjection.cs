using Microsoft.Extensions.DependencyInjection;
using Yact.Application.Services.Activities;
using Yact.Application.UseCases.Activities;

namespace Yact.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(UploadActivity).Assembly));

        // Automapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Services
        services.AddScoped<FullReadActivityService>();
        services.AddScoped<ClimbHandlerService>();

        return services;
    }
}
