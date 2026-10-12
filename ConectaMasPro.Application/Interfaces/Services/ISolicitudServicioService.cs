using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.Solicitudes;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface ISolicitudServicioService
    {
        Task<Result<SolicitudServicioListadoDto>> CrearAsync(Guid clienteId, CrearSolicitudServicioRequest request, CancellationToken cancellationToken = default);
        Task<Result<CambiarEstadoSolicitudResponse>> CambiarEstadoAsync(Guid solicitudId, Guid proveedorId, EstadoSolicitudServicio estado, CancellationToken cancellationToken = default);
        Task<Result> EstablecerPrecioAcordadoAsync(Guid solicitudId, Guid proveedorId, EstablecerPrecioAcordadoRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmarFinalizacionAsync(Guid solicitudId, Guid clienteId, ConfirmarFinalizacionRequest request, CancellationToken cancellationToken = default);

        Task<Result<PagedResponse<SolicitudesClienteResponse>>> ObtenerMisSolicitudesAsync(Guid clienteId, 
                                SolicitudServicioFiltroRequest filtro, CancellationToken cancellationToken = default);

        Task<Result<SolicitudServicioDetalleDto>> ObtenerDetalleAsync(Guid solicitudId,Guid usuarioId,
                                                  CancellationToken cancellationToken = default);

        Task<Result<PagedResponse<SolicitudesProveedorResponse>>>ObtenerSolicitudesRecibidasAsync(Guid proveedorId, SolicitudServicioFiltroRequest filtro,
            CancellationToken cancellationToken = default);
    }
}
