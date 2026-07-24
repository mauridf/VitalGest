using Microsoft.Extensions.Logging;
using Quartz;

namespace VitalGest.Infrastructure.BackgroundJobs;

/// <summary>
/// Job de teste que executa a cada minuto para verificar se o scheduler está funcionando.
/// Deve ser removido em produção.
/// </summary>
[DisallowConcurrentExecution]
public class HelloWorldJob : IJob
{
    private readonly ILogger<HelloWorldJob> _logger;

    public HelloWorldJob(ILogger<HelloWorldJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Hello World Job executado em: {Time}", DateTime.UtcNow);
        _logger.LogInformation("Próxima execução: {NextFireTime}", context.NextFireTimeUtc?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

        return Task.CompletedTask;
    }
}