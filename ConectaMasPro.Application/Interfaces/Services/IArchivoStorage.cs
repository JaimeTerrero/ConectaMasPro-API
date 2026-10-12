namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IArchivoStorage
    {
        Task<string> GuardarImagenAsync(
            byte[] contenido,
            string tipoContenido,
            string carpeta,
            CancellationToken cancellationToken = default);

        Task EliminarAsync(string? urlRelativa, CancellationToken cancellationToken = default);
    }
}
