using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VitalGest.Application.Mappings;

namespace VitalGest.Application.DependencyInjection;

/// <summary>
/// Registra todos os serviços da camada de Aplicação no container DI.
/// </summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ===== AutoMapper =====
        services.AddSingleton<IMapper>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, loggerFactory);
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

        // ===== FluentValidation =====
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // ===== Serviços de Aplicação =====
        // Serão registrados na FASE 7
        // services.AddScoped<IAuthService, AuthService>();
        // services.AddScoped<IClinicService, ClinicService>();
        // ... etc

        return services;
    }
}