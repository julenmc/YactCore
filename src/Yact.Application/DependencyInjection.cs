using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Yact.Application.Handlers.Activities.UploadActivity;
using Yact.Application.Mapping;

namespace Yact.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(UploadActivityHandler).Assembly));

        // Automapper
        IMapper mapper = ActivityMapper.RegisterMaps().CreateMapper();
        services.AddSingleton(mapper);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
