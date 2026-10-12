using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Notificaciones;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface INotificacionService
    {
        Task<Result<Guid>> CrearAsync(CrearNotificacionRequest request, CancellationToken cancellationToken = default);
        Task<Result> MarcarComoLeidaAsync(Guid notificacionId, CancellationToken cancellationToken = default);
    }
}
