using ConectaMasPro.Application.Common;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IRecompensaService
    {
        Task<Result> AcreditarPuntosPorServicioCompletadoAsync(
            SolicitudServicio solicitud,
            CancellationToken cancellationToken = default);
    }
}
