using Microsoft.Extensions.Logging;
using Quartz;
using VitalGest.Core.Interfaces;

namespace VitalGest.Infrastructure.BackgroundJobs;

/// <summary>
/// Job de limpeza de dados expirados.
/// Executa semanalmente (domingo às 03:00).
/// Remove:
/// - Refresh tokens expirados
/// - Logs de auditoria com mais de 90 dias
/// - Notificações lidas com mais de 30 dias
/// </summary>
[DisallowConcurrentExecution]
public class CleanupJob : IJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(IUnitOfWork uow, ILogger<CleanupJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando job de limpeza de dados...");

        try
        {
            var now = DateTime.UtcNow;
            var auditCutoff = now.AddDays(-90);
            var notificationCutoff = now.AddDays(-30);
            var refreshTokenCutoff = now.AddDays(-7);

            var cleanupStats = new { RefreshTokens = 0, AuditLogs = 0, Notifications = 0 };

            // 1. Limpa refresh tokens expirados (zera o campo)
            var usersWithExpiredTokens = await _uow.Users.FindAsync(u =>
                u.RefreshToken != null && u.RefreshTokenExpiryTime < now);

            foreach (var user in usersWithExpiredTokens)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _uow.Users.UpdateAsync(user);
                cleanupStats = cleanupStats with { RefreshTokens = cleanupStats.RefreshTokens + 1 };
            }

            // 2. Remove logs de auditoria antigos (> 90 dias)
            var oldAuditLogs = await _uow.AuditLogs.FindAsync(a => a.CreatedAt < auditCutoff);
            foreach (var log in oldAuditLogs)
            {
                await _uow.AuditLogs.DeleteAsync(log);
                cleanupStats = cleanupStats with { AuditLogs = cleanupStats.AuditLogs + 1 };
            }

            // 3. Remove notificações lidas antigas (> 30 dias)
            var oldNotifications = await _uow.Notifications.FindAsync(n =>
                n.IsRead && n.ReadAt < notificationCutoff);

            foreach (var notification in oldNotifications)
            {
                await _uow.Notifications.DeleteAsync(notification);
                cleanupStats = cleanupStats with { Notifications = cleanupStats.Notifications + 1 };
            }

            await _uow.SaveChangesAsync();

            _logger.LogInformation(
                "Job de limpeza concluído. Tokens: {Tokens}, Auditoria: {Audit}, Notificações: {Notif}",
                cleanupStats.RefreshTokens, cleanupStats.AuditLogs, cleanupStats.Notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de limpeza.");
            throw new JobExecutionException(ex, false);
        }
    }

    public static ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity("CleanupTrigger", "VitalGest")
            .WithCronSchedule("0 0 3 ? * SUN") // Todo domingo às 03:00
            .Build();
    }

    public static IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<CleanupJob>()
            .WithIdentity("CleanupJob", "VitalGest")
            .WithDescription("Limpa dados expirados: refresh tokens, auditoria, notificações")
            .StoreDurably()
            .Build();
    }
}