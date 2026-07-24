using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VitalGest.Application.Interfaces;
using VitalGest.Application.Services;
using AutoMapper;
using VitalGest.Application.Mappings;

namespace VitalGest.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ===== AutoMapper =====
        services.AddSingleton<IMapper>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), loggerFactory);
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

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
        services.AddScoped<IMedicalRecordService, MedicalRecordService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();
        services.AddScoped<IAtestService, AtestService>();
        services.AddScoped<IFinancialService, FinancialService>();
        services.AddScoped<IInsuranceService, InsuranceService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        // ===== Serviços de Infraestrutura (registrados na Application) =====
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}