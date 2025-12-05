namespace Yact.Infrastructure.Configuration;

public class FileStorageConfiguration
{
    public string BasePath { get; set; } = "";
    public long MaxFileSize { get; set; }
    public List<string> AllowedExtensions { get; set; } = new();
}
