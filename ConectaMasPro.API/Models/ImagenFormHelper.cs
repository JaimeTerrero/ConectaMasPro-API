using ConectaMasPro.Application.Common;

namespace ConectaMasPro.API.Models
{
    internal static class ImagenFormHelper
    {
        private static readonly HashSet<string> TiposPermitidos =
            ["image/jpeg", "image/png", "image/webp"];

        public static async Task<Result<byte[]?>> LeerAsync(
            IFormFile? archivo,
            long tamanoMaximo,
            CancellationToken cancellationToken)
        {
            if (archivo is null || archivo.Length == 0)
                return Result<byte[]?>.Ok(null);

            if (!TiposPermitidos.Contains(archivo.ContentType.ToLowerInvariant()))
                return Result<byte[]?>.Fail("Solo se permiten imágenes JPEG, PNG o WebP.");

            if (archivo.Length > tamanoMaximo)
                return Result<byte[]?>.Fail($"La imagen no puede superar {tamanoMaximo / 1024 / 1024} MB.");

            await using var memoria = new MemoryStream();
            await archivo.CopyToAsync(memoria, cancellationToken);
            var contenido = memoria.ToArray();

            if (!FirmaValida(contenido, archivo.ContentType))
                return Result<byte[]?>.Fail("El contenido del archivo no corresponde a una imagen válida.");

            return Result<byte[]?>.Ok(contenido);
        }

        public static string? NormalizarNombre(IFormFile? archivo)
        {
            if (archivo is null)
                return null;

            var nombre = Path.GetFileName(archivo.FileName);
            return nombre.Length <= 255 ? nombre : nombre[..255];
        }

        private static bool FirmaValida(byte[] bytes, string tipo) => tipo.ToLowerInvariant() switch
        {
            "image/jpeg" => bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
            "image/png" => bytes.Length >= 8 && bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            "image/webp" => bytes.Length >= 12 && bytes.AsSpan(0, 4).SequenceEqual("RIFF"u8) && bytes.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };
    }
}
