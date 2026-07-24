using Microsoft.Extensions.Logging;
using Quartz;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.BackgroundJobs;

/// <summary>
/// Job que envia lembretes de agendamento.
/// Executa a cada 30 minutos.
/// Notifica pacientes 24h e 1h antes do horário agendado.
/// </summary>
[DisallowConcurrentExecution]
public class AppointmentReminderJob : IJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AppointmentReminderJob> _logger;

    public AppointmentReminderJob(IUnitOfWork uow, ILogger<AppointmentReminderJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando job de lembretes de agendamento...");

        try
        {
            var now = DateTime.UtcNow;
            var nowTime = TimeOnly.FromDateTime(now);
            var today = DateOnly.FromDateTime(now);

            // Busca agendamentos para hoje que estão confirmados
            var todayAppointments = await _uow.Appointments.FindAsync(a =>
                a.AppointmentDate == today &&
                (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed) &&
                !a.IsConfirmed == false); // Ainda não confirmados totalmente

            var remindersSent = 0;

            foreach (var appointment in todayAppointments)
            {
                // Verifica se está a 1 hora do horário (com margem de 30 min do job)
                var timeUntilAppointment = appointment.StartTime.ToTimeSpan() - nowTime.ToTimeSpan();

                // Notifica 1h antes (entre 60min e 90min)
                if (timeUntilAppointment.TotalMinutes is >= 60 and <= 90)
                {
                    await SendReminder(appointment, "1 hora");
                    remindersSent++;
                }
                // Notifica 24h antes
                else if (timeUntilAppointment.TotalHours is >= 23.5 and <= 24.5)
                {
                    await SendReminder(appointment, "24 horas");
                    remindersSent++;
                }
            }

            _logger.LogInformation("Job de lembretes concluído. {Count} lembretes enviados.", remindersSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de lembretes de agendamento.");
            throw new JobExecutionException(ex, false); // Refire on next schedule
        }
    }

    private async Task SendReminder(Appointment appointment, string when)
    {
        // Cria notificação para o paciente (via sistema)
        var notification = new Notification
        {
            ClinicId = appointment.ClinicId,
            PatientId = appointment.PatientId,
            Title = "Lembrete de Consulta",
            Message = $"Você tem uma consulta agendada em {when}.\n" +
                      $"Data: {appointment.AppointmentDate:dd/MM/yyyy}\n" +
                      $"Horário: {appointment.StartTime:HH:mm}\n" +
                      $"Médico: Dr(a). {appointment.Doctor?.Name ?? "N/A"}",
            Type = NotificationType.AppointmentReminder,
            Channel = "in-app",
            SentAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification);

        _logger.LogInformation("Lembrete enviado: AppointmentId={AppointmentId}, When={When}",
            appointment.Id, when);
    }

    /// <summary>
    /// Configuração do trigger: a cada 30 minutos.
    /// </summary>
    public static ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity("AppointmentReminderTrigger", "VitalGest")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(30)
                .RepeatForever())
            .Build();
    }

    /// <summary>
    /// Configuração do job.
    /// </summary>
    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<AppointmentReminderJob>()
            .WithIdentity("AppointmentReminderJob", "VitalGest")
            .WithDescription("Envia lembretes de agendamento 24h e 1h antes")
            .StoreDurably()
            .Build();
    }
}