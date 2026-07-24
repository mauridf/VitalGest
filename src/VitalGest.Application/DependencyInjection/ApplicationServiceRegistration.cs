using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using VitalGest.Application.Interfaces;
using VitalGest.Application.Services;

namespace VitalGest.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ===== AutoMapper =====
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // ===== FluentValidation =====
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation(options =>
        {
            options.DisableDataAnnotationsValidation = true;
        });

        // ===== Serviços de Aplicação =====
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClinicService, ClinicService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IScheduleService, ScheduleService>();

        return services;
    }
}