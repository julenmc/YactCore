using Microsoft.Extensions.DependencyInjection;
using Yact.Application.Handlers.Activities;

namespace Yact.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(UploadActivityHandler).Assembly));

        // Automapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
