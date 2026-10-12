using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Dashboard;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<Result<ClienteDashboardDto>> ObtenerDashboardClienteAsync(
            Guid clienteId,
            CancellationToken cancellationToken = default);

        Task<Result<ProveedorDashboardDto>> ObtenerDashboardProveedorAsync(
            Guid proveedorId,
            CancellationToken cancellationToken = default);
    }
}
