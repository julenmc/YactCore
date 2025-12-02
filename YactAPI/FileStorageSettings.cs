namespace YactAPI
{
    public class FileStorageSettings
    {
        public string BasePath { get; set; } = "";
        public long MaxFileSize { get; set; }
        public List<string> AllowedExtensions { get; set; } = new();
    }
}
