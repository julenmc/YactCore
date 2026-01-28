using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yact.Application.Interfaces.Files;

namespace Yact.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
            IOptions<FileStorageConfiguration> config,
            ILogger<LocalFileStorageService> logger)
    {
        _basePath = config.Value.BasePath;
        _logger = logger;

        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string folder)
    {
        // Generate unique name to avoid collisions
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var folderPath = Path.Combine(_basePath, folder);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueFileName);

        using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStreamOutput);
        }

        return filePath;
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
