using Microsoft.Extensions.Logging;
using Quartz;
using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.BackgroundJobs;

/// <summary>
/// Job que gera slots de horário para os próximos 30 dias.
/// Executa diariamente à 01:00.
/// Baseia-se nas regras de Schedule de cada médico.
/// </summary>
[DisallowConcurrentExecution]
public class GenerateTimeSlotsJob : IJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GenerateTimeSlotsJob> _logger;

    public GenerateTimeSlotsJob(IUnitOfWork uow, ILogger<GenerateTimeSlotsJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando job de geração de slots de horário...");

        try
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(30);

            // Busca todas as clínicas ativas
            var clinics = await _uow.Clinics.FindAsync(c => c.IsActive);
            var generatedCount = 0;

            foreach (var clinic in clinics)
            {
                // Busca todos os médicos com regras de agenda na clínica
                var schedules = await _uow.Schedules.FindAsync(s =>
                    s.ClinicId == clinic.Id && s.IsActive);

                var doctorIds = schedules.Select(s => s.DoctorUserId).Distinct();

                foreach (var doctorId in doctorIds)
                {
                    try
                    {
                        await _uow.Schedules.GenerateSlotsAsync(
                            doctorId, today, endDate, clinic.Id);
                        generatedCount++;
                        _logger.LogDebug("Slots gerados para DoctorId={DoctorId}, ClinicId={ClinicId}",
                            doctorId, clinic.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Erro ao gerar slots para DoctorId={DoctorId}, ClinicId={ClinicId}",
                            doctorId, clinic.Id);
                    }
                }
            }

            await _uow.SaveChangesAsync();

            _logger.LogInformation("Job de geração de slots concluído. {Count} médicos processados.", generatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de geração de slots.");
            throw new JobExecutionException(ex, false);
        }
    }

    public static ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity("GenerateTimeSlotsTrigger", "VitalGest")
            .WithCronSchedule("0 0 1 * * ?") // Todo dia à 01:00
            .Build();
    }

    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<GenerateTimeSlotsJob>()
            .WithIdentity("GenerateTimeSlotsJob", "VitalGest")
            .WithDescription("Gera slots de horário para os próximos 30 dias")
            .StoreDurably()
            .Build();
    }
}