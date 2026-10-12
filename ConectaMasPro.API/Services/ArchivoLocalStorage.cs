using ConectaMasPro.Application.Interfaces.Services;

namespace ConectaMasPro.API.Services
{
    public class ArchivoLocalStorage : IArchivoStorage
    {
        private readonly string _raizUploads;

        public ArchivoLocalStorage(IWebHostEnvironment environment)
        {
            var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            _raizUploads = Path.GetFullPath(Path.Combine(webRoot, "uploads"));
        }

        public async Task<string> GuardarImagenAsync(
            byte[] contenido,
            string tipoContenido,
            string carpeta,
            CancellationToken cancellationToken = default)
        {
            var extension = tipoContenido.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => throw new InvalidOperationException("El tipo de imagen no está permitido.")
            };

            var carpetaSegura = new string(carpeta.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(carpetaSegura))
                throw new InvalidOperationException("La carpeta de almacenamiento no es válida.");

            var directorio = Path.Combine(_raizUploads, carpetaSegura);
            Directory.CreateDirectory(directorio);

            var nombre = $"{Guid.NewGuid():N}{extension}";
            var ruta = Path.Combine(directorio, nombre);
            await File.WriteAllBytesAsync(ruta, contenido, cancellationToken);

            return $"/uploads/{carpetaSegura}/{nombre}";
        }

        public Task EliminarAsync(string? urlRelativa, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(urlRelativa) || !urlRelativa.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            var rutaRelativa = urlRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var ruta = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(_raizUploads)!, rutaRelativa));

            if (ruta.StartsWith(_raizUploads + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && File.Exists(ruta))
                File.Delete(ruta);

            return Task.CompletedTask;
        }
    }
}
