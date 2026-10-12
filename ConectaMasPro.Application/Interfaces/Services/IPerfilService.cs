using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Perfiles;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IPerfilService
    {
        Task<Result<PerfilClienteDto>> ObtenerPerfilClienteAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<Result<PerfilProveedorDto>> ObtenerPerfilProveedorAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<Result<PerfilPublicoDto>> ObtenerPerfilPublicoClienteAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<Result<PerfilPublicoDto>> ObtenerPerfilPublicoProveedorAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<Result<PerfilActualizadoDto>> ActualizarMiPerfilAsync(
            Guid usuarioId,
            ActualizarPerfilRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<FotoPerfilDto>> ActualizarFotoPerfilAsync(
            Guid usuarioId,
            ActualizarFotoPerfilRequest request,
            CancellationToken cancellationToken = default);
    }
}
