using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VitalGest.Infrastructure.Data.Migrations;

/// <summary>
/// Inicializador do banco de dados usando DbUp.
/// Lê scripts SQL do sistema de arquivos e executa em ordem.
/// </summary>
public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
        _logger = logger;
    }

    /// <summary>
    /// Executa todas as migrações pendentes.
    /// Procura scripts na pasta db/migrations relativa ao diretório da aplicação.
    /// </summary>
    public void RunMigrations()
    {
        _logger.LogInformation("Iniciando migrações do banco de dados...");

        // Garante que o banco existe
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        // Determina o caminho da pasta de migrações
        // Em desenvolvimento: procura relativo ao diretório do projeto
        // Em produção: os scripts são copiados para o output
        var migrationsPath = FindMigrationsPath();
        _logger.LogInformation("Procurando scripts de migração em: {Path}", migrationsPath);

        if (!Directory.Exists(migrationsPath))
        {
            throw new DirectoryNotFoundException(
                $"Pasta de migrações não encontrada: {migrationsPath}. " +
                "Certifique-se de que a pasta 'db/migrations' existe e os scripts SQL estão presentes.");
        }

        // Configura o DbUp com scripts do sistema de arquivos
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsFromFileSystem(migrationsPath)
            .WithTransactionPerScript()
            .LogToConsole()
            .LogTo(new DbUpLogger(_logger))
            .Build();

        // Verifica se há migrações pendentes
        if (!upgrader.IsUpgradeRequired())
        {
            _logger.LogInformation("Nenhuma migração pendente. Banco de dados está atualizado.");
            return;
        }

        // Lista scripts que serão executados
        var scriptsToRun = upgrader.GetScriptsToExecute();
        _logger.LogInformation("{Count} script(s) pendente(s) para execução.", scriptsToRun.Count);
        foreach (var script in scriptsToRun)
        {
            _logger.LogInformation("  → {ScriptName}", script.Name);
        }

        // Executa as migrações
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Erro ao executar migrações do banco de dados.");
            throw new InvalidOperationException("Falha na migração do banco de dados.", result.Error);
        }

        _logger.LogInformation("Migrações concluídas com sucesso! {Count} script(s) executado(s).", scriptsToRun.Count);
    }

    /// <summary>
    /// Encontra o caminho da pasta de migrações.
    /// Verifica múltiplos locais possíveis (desenvolvimento vs produção).
    /// </summary>
    private static string FindMigrationsPath()
    {
        // 1. Caminho relativo ao diretório de execução (produção)
        var basePath = AppContext.BaseDirectory;
        var relativePath = Path.Combine(basePath, "db", "migrations");
        if (Directory.Exists(relativePath))
            return relativePath;

        // 2. Caminho relativo ao projeto (desenvolvimento - sobe 4 níveis até a raiz)
        var devPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "..", "db", "migrations"));
        if (Directory.Exists(devPath))
            return devPath;

        // 3. Fallback: retorna o caminho relativo (vai falhar com erro claro)
        return relativePath;
    }
}

/// <summary>
/// Logger adapter para integrar DbUp com Microsoft.Extensions.Logging.
/// </summary>
internal class DbUpLogger : DbUp.Engine.Output.IUpgradeLog
{
    private readonly ILogger<DatabaseInitializer> _logger;

    public DbUpLogger(ILogger<DatabaseInitializer> logger)
    {
        _logger = logger;
    }

    public void LogTrace(string format, params object[] args)
    {
        _logger.LogTrace(format, args);
    }

    public void LogDebug(string format, params object[] args)
    {
        _logger.LogDebug(format, args);
    }

    public void LogInformation(string format, params object[] args)
    {
        _logger.LogInformation(format, args);
    }

    public void LogWarning(string format, params object[] args)
    {
        _logger.LogWarning(format, args);
    }

    public void LogError(string format, params object[] args)
    {
        _logger.LogError(format, args);
    }

    public void LogError(Exception exception, string format, params object[] args)
    {
        _logger.LogError(exception, format, args);
    }

    public void WriteInformation(string format, params object[] args)
    {
        _logger.LogInformation(format, args);
    }

    public void WriteError(string format, params object[] args)
    {
        _logger.LogError(format, args);
    }

    public void WriteWarning(string format, params object[] args)
    {
        _logger.LogWarning(format, args);
    }
}