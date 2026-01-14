namespace Yact.Application.Interfaces.Files;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream stream, string fileName, string folder);
    Task<Stream> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
}
