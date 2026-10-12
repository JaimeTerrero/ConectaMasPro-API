using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.Servicios;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IServicioService
    {
        Task<Result<ServicioListadoDto>> CrearAsync(
            Guid usuarioId,
            CrearServicioRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<ServicioDto>>> ObtenerActivosAsync(
            CancellationToken cancellationToken = default);

        Task<Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>> ExplorarServiciosAsync(ServicioFiltroRequest filtro,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>> ObtenerMisServiciosAsync(
            Guid usuarioId,
            ServicioFiltroRequest filtro,
            CancellationToken cancellationToken = default);

        Task<Result<ServicioDetalleDto>> ObtenerDetalleAsync(
            Guid servicioId,
            CancellationToken cancellationToken = default);
    }
}
