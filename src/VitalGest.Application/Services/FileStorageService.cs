using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VitalGest.Application.Interfaces;

namespace VitalGest.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _basePath = Path.Combine(AppContext.BaseDirectory, "uploads");
        _logger = logger;

        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subfolder, CancellationToken ct = default)
    {
        var folderPath = Path.Combine(_basePath, subfolder);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(folderPath, uniqueFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        _logger.LogInformation("Arquivo salvo: {FilePath}", filePath);

        return $"/uploads/{subfolder}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_basePath, fileUrl.TrimStart('/').Replace("uploads/", ""));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("Arquivo removido: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }
}
