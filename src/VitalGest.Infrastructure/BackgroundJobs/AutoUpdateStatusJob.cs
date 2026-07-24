using Microsoft.Extensions.Logging;
using Quartz;
using VitalGest.Core.Enums;
using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.BackgroundJobs;

/// <summary>
/// Job que atualiza automaticamente o status de agendamentos.
/// Executa diariamente à 00:30.
/// Marca como NÃO_COMPARECEU agendamentos do dia anterior que não foram realizados.
/// </summary>
[DisallowConcurrentExecution]
public class AutoUpdateStatusJob : IJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AutoUpdateStatusJob> _logger;

    public AutoUpdateStatusJob(IUnitOfWork uow, ILogger<AutoUpdateStatusJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando job de atualização automática de status...");

        try
        {
            var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Busca agendamentos de ontem que ficaram como Scheduled ou Confirmed
            var pendingAppointments = await _uow.Appointments.FindAsync(a =>
                a.AppointmentDate == yesterday &&
                (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed));

            var updatedCount = 0;

            foreach (var appointment in pendingAppointments)
            {
                appointment.Status = AppointmentStatus.NoShow;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _uow.Appointments.UpdateAsync(appointment);
                updatedCount++;

                _logger.LogInformation("Agendamento {AppointmentId} marcado como NoShow automaticamente.", appointment.Id);
            }

            if (updatedCount > 0)
            {
                await _uow.SaveChangesAsync();
            }

            _logger.LogInformation("Job de atualização concluído. {Count} agendamentos atualizados para NoShow.", updatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de atualização de status.");
            throw new JobExecutionException(ex, false);
        }
    }

    public static ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity("AutoUpdateStatusTrigger", "VitalGest")
            .WithCronSchedule("0 30 0 * * ?") // Todo dia às 00:30
            .Build();
    }

    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<AutoUpdateStatusJob>()
            .WithIdentity("AutoUpdateStatusJob", "VitalGest")
            .WithDescription("Marca como NoShow agendamentos do dia anterior não finalizados")
            .StoreDurably()
            .Build();
    }
}