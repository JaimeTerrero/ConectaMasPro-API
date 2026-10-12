using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Conversaciones;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IConversacionService
    {
        Task<Result<Guid>> CrearAsync(CrearConversacionRequest request, Guid usuarioId, CancellationToken cancellationToken = default);
        Task<Result<MensajeDto>> EnviarMensajeAsync(EnviarMensajeRequest request, Guid emisorId, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<ConversacionDto>>> ObtenerConversacionesAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<MensajeDto>>> ObtenerMensajesAsync(Guid conversacionId, Guid usuarioId, CancellationToken cancellationToken = default);
        Task<bool> EsParticipanteAsync(Guid conversacionId, Guid usuarioId, CancellationToken cancellationToken = default);
    }
}
