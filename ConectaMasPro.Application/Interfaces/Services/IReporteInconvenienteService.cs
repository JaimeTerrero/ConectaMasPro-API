using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.ReportesInconvenientes;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IReporteInconvenienteService
    {
        Task<Result<ReporteInconvenienteDto>> CrearAsync(Guid reportanteId, CrearReporteInconvenienteRequest request, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>> ObtenerMisReportesAsync(Guid reportanteId, ReporteInconvenienteFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<Result<ReporteInconvenienteDto>> ObtenerDetalleAsync(Guid reporteId, Guid usuarioId, bool esAdministrador, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>> ObtenerTodosAsync(Guid administradorId, ReporteInconvenienteFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<Result<ReporteInconvenienteDto>> ActualizarEstadoAsync(Guid reporteId, Guid administradorId, ActualizarEstadoReporteRequest request, CancellationToken cancellationToken = default);
        Task<Result<EvidenciaReporteDto>> ActualizarEvidenciaAsync(Guid reporteId, Guid reportanteId, ActualizarEvidenciaReporteRequest request, CancellationToken cancellationToken = default);
    }
}
