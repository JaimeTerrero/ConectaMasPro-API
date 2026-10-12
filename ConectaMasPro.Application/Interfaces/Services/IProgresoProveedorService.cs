using ConectaMasPro.Application.Common;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IProgresoProveedorService
    {
        Task<Result> RegistrarServicioCompletadoAsync(
            SolicitudServicio solicitud,
            CancellationToken cancellationToken = default);
    }
}
