using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Interfaces;
using VitalGest.Infrastructure.Data.Context;
using VitalGest.Infrastructure.Data.Migrations;
using VitalGest.Infrastructure.Data.Repositories;
using VitalGest.Infrastructure.Services;

namespace VitalGest.Infrastructure.DependencyInjection;

/// <summary>
/// Registra todos os serviços da camada de Infraestrutura no container DI.
/// </summary>
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== Banco de Dados (PostgreSQL + EF Core) =====
        services.AddDbContext<VitalGestDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Em desenvolvimento, habilita logging detalhado
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // ===== Migrations (DbUp) =====
        services.AddTransient<DatabaseInitializer>();

        // ===== Redis Cache =====
        var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString")
            ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configOptions = ConfigurationOptions.Parse(redisConnectionString);
            configOptions.AbortOnConnectFail = false; // Não falha se Redis estiver indisponível
            configOptions.ConnectRetry = 3;
            configOptions.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(configOptions);
        });

        services.AddSingleton<ICacheService, CacheService>();

        // ===== Tenant Service (Scoped - um por requisição) =====
        services.AddScoped<ITenantService, TenantService>();

        // ===== Repositórios =====
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();

        // ===== Unit of Work =====
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ===== Serviços de Infraestrutura =====
        services.AddScoped<FileStorageService>();

        return services;
    }
}