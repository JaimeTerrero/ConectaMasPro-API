using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Pagos;
using ConectaMasPro.Application.Common.Pagination;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IPagoService
    {
        Result<ConfiguracionPublicaStripeDto> ObtenerConfiguracionPublica();
        Task<Result<ResumenPagoDto>> ObtenerResumenAsync(Guid solicitudId, Guid clienteId, CancellationToken cancellationToken = default);
        Task<Result<IntencionPagoDto>> CrearIntencionAsync(Guid solicitudId, Guid clienteId, CancellationToken cancellationToken = default);
        Task<Result<PagoDetalleDto>> SincronizarAsync(Guid pagoId, Guid clienteId, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<GananciasProveedorResponse>>> ObtenerGananciasProveedorAsync(Guid proveedorId, GananciasProveedorFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<HistorialPagosClienteResponse>>> ObtenerHistorialPagosClienteAsync(Guid clienteId, HistorialPagosClienteFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<Result> ProcesarWebhookAsync(string contenido, string firma, CancellationToken cancellationToken = default);
    }
}
