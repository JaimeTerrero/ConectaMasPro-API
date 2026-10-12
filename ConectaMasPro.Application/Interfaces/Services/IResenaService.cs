using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Resenas;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IResenaService
    {
        Task<Result<ResenaDto>> CrearAsync(
            Guid evaluadorId,
            CrearResenaRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<ResumenCalificacionesDto>> ObtenerPorUsuarioAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default);

        Task<Result<EstadoCalificacionSolicitudDto>> ObtenerEstadoSolicitudAsync(
            Guid solicitudServicioId,
            Guid usuarioId,
            CancellationToken cancellationToken = default);
    }
}
