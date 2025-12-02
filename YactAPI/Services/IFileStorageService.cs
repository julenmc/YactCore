namespace YactAPI.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFile(IFormFile archivo);
        Task<byte[]> GetFile(string nombreArchivo);
        Task DeleteFile(string nombreArchivo);
    }
}
