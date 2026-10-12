using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Notificaciones;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> CrearAsync(CrearNotificacionRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Titulo))
                return Result<Guid>.Fail("El título es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Mensaje))
                return Result<Guid>.Fail("El mensaje es obligatorio.");

            var notificacion = new Notificacion
            {
                UsuarioId = request.UsuarioId,
                Titulo = request.Titulo.Trim(),
                Mensaje = request.Mensaje.Trim(),
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Ok(notificacion.Id);
        }

        public async Task<Result> MarcarComoLeidaAsync(Guid notificacionId, CancellationToken cancellationToken = default)
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>()
                .GetByIdAsync(notificacionId, cancellationToken);

            if (notificacion is null || !notificacion.Activo)
                return Result.Fail("Notificación no encontrada.");

            notificacion.Leida = true;
            notificacion.FechaModificacion = DateTime.UtcNow;

            _unitOfWork.Repository<Notificacion>().Update(notificacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
