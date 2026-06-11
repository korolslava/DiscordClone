using DiscordClone.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordClone.Infrastructure.Storage;

public sealed class LocalStorageService : IStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(IConfiguration config,
        ILogger<LocalStorageService> logger)
    {
        _uploadPath = config["Storage:LocalPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _baseUrl = config["Storage:BaseUrl"] ?? "http://localhost:5000/uploads";
        _logger = logger;

        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName,
        string contentType, CancellationToken ct = default)
    {
        var unique = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var path = Path.Combine(_uploadPath, unique);

        await using var output = File.Create(path);
        await fileStream.CopyToAsync(output, ct);

        _logger.LogInformation("Uploaded file: {FileName}", unique);
        return $"{_baseUrl}/{unique}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var path = Path.Combine(_uploadPath, fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            _logger.LogInformation("Deleted file: {FileName}", fileName);
        }

        return Task.CompletedTask;
    }
}