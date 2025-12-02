using Microsoft.Extensions.Options;

namespace YactAPI.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(
            IOptions<FileStorageSettings> settings,
            ILogger<FileStorageService> logger)
        {
            _basePath = settings.Value.BasePath;
            _logger = logger;

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveFile(IFormFile archivo)
        {
            var nombreUnico = $"{Guid.NewGuid()}_{archivo.FileName}";
            var rutaCompleta = Path.Combine(_basePath, nombreUnico);

            try
            {
                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await archivo.CopyToAsync(stream);

                _logger.LogInformation($"Archivo guardado: {nombreUnico}");
                return nombreUnico;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar archivo: {archivo.FileName}");
                throw;
            }
        }

        public async Task<byte[]> GetFile(string nombreArchivo)
        {
            var rutaCompleta = Path.Combine(_basePath, nombreArchivo);

            if (!File.Exists(rutaCompleta))
                return null;

            return await File.ReadAllBytesAsync(rutaCompleta);
        }

        public Task DeleteFile(string nombreArchivo)
        {
            var rutaCompleta = Path.Combine(_basePath, nombreArchivo);

            if (File.Exists(rutaCompleta))
                File.Delete(rutaCompleta);

            return Task.CompletedTask;
        }
    }
}
