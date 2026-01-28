namespace Yact.Infrastructure.FileStorage;

public class FileStorageConfiguration
{
    public const string SectionName = "FileStorage";
    public string BasePath { get; set; } = "";
    public long MaxFileSize { get; set; }
    public List<string> AllowedExtensions { get; set; } = new();
}
