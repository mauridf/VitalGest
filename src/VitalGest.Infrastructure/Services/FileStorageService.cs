using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VitalGest.Infrastructure.Services;

/// <summary>
/// Serviço de armazenamento de arquivos (documentos, anexos de exames, etc.).
/// Suporta armazenamento local e pode ser estendido para S3/MinIO.
/// </summary>
public class FileStorageService
{
    private readonly string _basePath;
    private readonly long _maxFileSize;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _basePath = configuration.GetValue<string>("Storage:BasePath") ?? "uploads";
        _maxFileSize = configuration.GetValue<long?>("Storage:MaxFileSize") ?? 10_485_760; // 10MB
        _logger = logger;
    }

    /// <summary>
    /// Salva um arquivo no disco.
    /// </summary>
    /// <param name="fileName">Nome original do arquivo</param>
    /// <param name="content">Conteúdo binário do arquivo</param>
    /// <param name="subFolder">Subpasta (ex: documents, exams)</param>
    /// <returns>URL relativa do arquivo salvo</returns>
    public async Task<string> SaveAsync(string fileName, byte[] content, string subFolder = "documents")
    {
        if (content.Length > _maxFileSize)
            throw new InvalidOperationException($"Arquivo excede o tamanho máximo de {_maxFileSize / 1_048_576}MB.");

        var uploadDir = Path.Combine(_basePath, subFolder);
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid():N}_{fileName}";
        var filePath = Path.Combine(uploadDir, uniqueName);

        await File.WriteAllBytesAsync(filePath, content);

        _logger.LogInformation("Arquivo salvo: {Path}", filePath);
        return Path.Combine(subFolder, uniqueName);
    }

    /// <summary>
    /// Exclui um arquivo do disco.
    /// </summary>
    public Task DeleteAsync(string fileUrl)
    {
        var fullPath = Path.Combine(_basePath, fileUrl);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Arquivo excluído: {Path}", fullPath);
        }
        return Task.CompletedTask;
    }
}
