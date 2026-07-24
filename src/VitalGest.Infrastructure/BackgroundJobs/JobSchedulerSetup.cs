using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace VitalGest.Infrastructure.BackgroundJobs;

public static class JobSchedulerSetup
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = "VitalGest-Scheduler";
            q.SchedulerId = "AUTO";

            q.UseDefaultThreadPool(tp => tp.MaxConcurrency = 10);
            q.UseInMemoryStore();

            // 0. Hello World (a cada 1 minuto) - apenas para debug
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                q.AddJob<HelloWorldJob>(job => job
                    .WithIdentity("HelloWorldJob", "VitalGest")
                    .WithDescription("Job de teste - executa a cada minuto")
                    .StoreDurably());

                q.AddTrigger(t => t
                    .WithIdentity("HelloWorldTrigger", "VitalGest")
                    .ForJob("HelloWorldJob", "VitalGest")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(1)
                        .RepeatForever()));
            }

            // 1. Lembrete de Agendamentos (a cada 30 min)
            q.AddJob<AppointmentReminderJob>(job => job
                .WithIdentity("AppointmentReminderJob", "VitalGest")
                .WithDescription("Envia lembretes de agendamento 24h e 1h antes")
                .StoreDurably());

            q.AddTrigger(t => t
                .WithIdentity("AppointmentReminderTrigger", "VitalGest")
                .ForJob("AppointmentReminderJob", "VitalGest")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(30)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionFireNow()));

            // 2. Atualização Automática de Status (diário 00:30)
            q.AddJob<AutoUpdateStatusJob>(job => job
                .WithIdentity("AutoUpdateStatusJob", "VitalGest")
                .WithDescription("Marca como NoShow agendamentos do dia anterior não finalizados")
                .StoreDurably());

            q.AddTrigger(t => t
                .WithIdentity("AutoUpdateStatusTrigger", "VitalGest")
                .ForJob("AutoUpdateStatusJob", "VitalGest")
                .WithCronSchedule("0 30 0 * * ?", x => x
                    .WithMisfireHandlingInstructionFireAndProceed()));

            // 3. Geração de Slots (diário 01:00)
            q.AddJob<GenerateTimeSlotsJob>(job => job
                .WithIdentity("GenerateTimeSlotsJob", "VitalGest")
                .WithDescription("Gera slots de horário para os próximos 30 dias")
                .StoreDurably());

            q.AddTrigger(t => t
                .WithIdentity("GenerateTimeSlotsTrigger", "VitalGest")
                .ForJob("GenerateTimeSlotsJob", "VitalGest")
                .WithCronSchedule("0 0 1 * * ?", x => x
                    .WithMisfireHandlingInstructionFireAndProceed()));

            // 4. Limpeza de Dados (semanal domingo 03:00)
            q.AddJob<CleanupJob>(job => job
                .WithIdentity("CleanupJob", "VitalGest")
                .WithDescription("Limpa dados expirados: refresh tokens, auditoria, notificações")
                .StoreDurably());

            q.AddTrigger(t => t
                .WithIdentity("CleanupTrigger", "VitalGest")
                .ForJob("CleanupJob", "VitalGest")
                .WithCronSchedule("0 0 3 ? * SUN", x => x
                    .WithMisfireHandlingInstructionFireAndProceed()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
