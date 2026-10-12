using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class RecompensaService : IRecompensaService
    {
        private const int PuntosPorServicioCompletado = 100;

        private readonly IUnitOfWork _unitOfWork;

        public RecompensaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AcreditarPuntosPorServicioCompletadoAsync(
    SolicitudServicio solicitud,
    CancellationToken cancellationToken = default)
        {
            if (solicitud is null)
            {
                return Result.Fail(
                    "La solicitud de servicio no es válida.");
            }

            if (solicitud.ClienteId == Guid.Empty)
            {
                return Result.Fail(
                    "La solicitud no posee un cliente válido.");
            }

            var movimientosExistentes = await _unitOfWork
                .Repository<MovimientoRecompensa>()
                .FindAsync(
                    x =>
                        x.SolicitudServicioId == solicitud.Id &&
                        x.Activo,
                    cancellationToken);

            var movimientoExistente =
                movimientosExistentes.FirstOrDefault();

            if (movimientoExistente is not null)
            {
                return Result.Ok();
            }

            var recompensas = await _unitOfWork
                .Repository<UsuarioRecompensa>()
                .FindAsync(
                    x =>
                        x.UsuarioId == solicitud.ClienteId &&
                        x.Activo,
                    cancellationToken);

            var usuarioRecompensa =
                recompensas.FirstOrDefault();

            if (usuarioRecompensa is null)
            {
                usuarioRecompensa = new UsuarioRecompensa
                {
                    UsuarioId = solicitud.ClienteId,
                    PuntosActuales = 0,
                    PuntosHistoricos = 0
                };

                await _unitOfWork
                    .Repository<UsuarioRecompensa>()
                    .AddAsync(
                        usuarioRecompensa,
                        cancellationToken);
            }

            var saldoAnterior =
                usuarioRecompensa.PuntosActuales;

            usuarioRecompensa.PuntosActuales +=
                PuntosPorServicioCompletado;

            usuarioRecompensa.PuntosHistoricos +=
                PuntosPorServicioCompletado;

            usuarioRecompensa.FechaModificacion =
                DateTime.UtcNow;

            /*
             * Si la cuenta ya existía, necesitamos
             * marcarla como modificada.
             */
            if (recompensas.Any())
            {
                _unitOfWork
                    .Repository<UsuarioRecompensa>()
                    .Update(usuarioRecompensa);
            }

            var movimiento = new MovimientoRecompensa
            {
                UsuarioRecompensaId =
                    usuarioRecompensa.Id,

                UsuarioId =
                    solicitud.ClienteId,

                SolicitudServicioId =
                    solicitud.Id,

                TipoMovimiento =
                    TipoMovimientoRecompensa.Credito,

                Puntos =
                    PuntosPorServicioCompletado,

                SaldoAnterior =
                    saldoAnterior,

                SaldoPosterior =
                    usuarioRecompensa.PuntosActuales,

                Concepto =
                    "Puntos por servicio completado"
            };

            await _unitOfWork
                .Repository<MovimientoRecompensa>()
                .AddAsync(
                    movimiento,
                    cancellationToken);

            return Result.Ok();
        }
    }
}
