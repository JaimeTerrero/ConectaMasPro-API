using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Resenas;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class ResenaService : IResenaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResenaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ResenaDto>> CrearAsync(
            Guid evaluadorId,
            CrearResenaRequest request,
            CancellationToken cancellationToken = default)
        {
            if (evaluadorId == Guid.Empty)
                return Result<ResenaDto>.Fail("El usuario autenticado no es válido.");

            if (request.SolicitudServicioId == Guid.Empty)
                return Result<ResenaDto>.Fail("La solicitud de servicio es obligatoria.");

            if (request.CalificacionGeneral < 1 || request.CalificacionGeneral > 5)
                return Result<ResenaDto>.Fail("La calificación debe estar entre 1 y 5 estrellas.");

            if (request.Comentario?.Trim().Length > 1000)
                return Result<ResenaDto>.Fail("El comentario no puede superar los 1000 caracteres.");

            var solicitudes = await _unitOfWork.Repository<SolicitudServicio>()
                .GetAsync(
                    x => x.Id == request.SolicitudServicioId && x.Activo,
                    cancellationToken: cancellationToken,
                    includes: [x => x.Cliente, x => x.Servicio, x => x.Servicio.Usuario]);

            var solicitud = solicitudes.FirstOrDefault();

            if (solicitud is null)
                return Result<ResenaDto>.Fail("La solicitud no existe.");

            if (solicitud.Estado != EstadoSolicitudServicio.Completada)
                return Result<ResenaDto>.Fail("Solo se puede calificar una solicitud completada.");

            var proveedorId = solicitud.Servicio.UsuarioId;
            var evaluado = evaluadorId switch
            {
                var id when id == solicitud.ClienteId => solicitud.Servicio.Usuario,
                var id when id == proveedorId => solicitud.Cliente,
                _ => null
            };

            if (evaluado is null)
                return Result<ResenaDto>.Fail("No tienes permiso para calificar esta solicitud.");

            var resenasSolicitud = await _unitOfWork.Repository<Resena>()
                .FindAsync(
                    x => x.SolicitudServicioId == solicitud.Id &&
                         x.EvaluadorId == evaluadorId &&
                         x.Activo,
                    cancellationToken);

            if (resenasSolicitud.Count > 0)
                return Result<ResenaDto>.Fail("Ya calificaste a este usuario por esta solicitud.");

            var resena = new Resena
            {
                EvaluadorId = evaluadorId,
                EvaluadoId = evaluado.Id,
                SolicitudServicioId = request.SolicitudServicioId,
                CalificacionGeneral = request.CalificacionGeneral,
                Comentario = request.Comentario?.Trim()
            };

            await _unitOfWork.Repository<Resena>().AddAsync(resena, cancellationToken);

            var resenasAnteriores = await _unitOfWork.Repository<Resena>()
                .FindAsync(x => x.EvaluadoId == evaluado.Id && x.Activo, cancellationToken);

            var perfiles = await _unitOfWork.Repository<PerfilUsuario>()
                .GetTrackedAsync(x => x.UsuarioId == evaluado.Id, cancellationToken: cancellationToken);

            var perfil = perfiles.FirstOrDefault();
            var totalResenas = resenasAnteriores.Count + 1;
            var sumaCalificaciones = resenasAnteriores.Sum(x => x.CalificacionGeneral) + resena.CalificacionGeneral;
            var promedio = Math.Round((decimal)sumaCalificaciones / totalResenas, 2);

            if (perfil is null)
            {
                perfil = new PerfilUsuario
                {
                    UsuarioId = evaluado.Id,
                    CalificacionPromedio = promedio,
                    TotalResenas = totalResenas
                };

                await _unitOfWork.Repository<PerfilUsuario>().AddAsync(perfil, cancellationToken);
            }
            else
            {
                perfil.CalificacionPromedio = promedio;
                perfil.TotalResenas = totalResenas;
                perfil.FechaModificacion = DateTime.UtcNow;
                _unitOfWork.Repository<PerfilUsuario>().Update(perfil);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ResenaDto>.Ok(
                CrearDto(resena, evaluadorId == solicitud.ClienteId ? solicitud.Cliente : solicitud.Servicio.Usuario),
                "La calificación fue registrada correctamente.");
        }

        public async Task<Result<ResumenCalificacionesDto>> ObtenerPorUsuarioAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId, cancellationToken);

            if (usuario is null || !usuario.Activo)
                return Result<ResumenCalificacionesDto>.Fail("El usuario no existe.");

            var resenas = await _unitOfWork.Repository<Resena>()
                .GetAsync(
                    x => x.EvaluadoId == usuarioId && x.Activo,
                    query => query.OrderByDescending(x => x.FechaCreacion),
                    cancellationToken,
                    x => x.Evaluador);

            var resumen = new ResumenCalificacionesDto
            {
                UsuarioId = usuarioId,
                CalificacionPromedio = resenas.Count == 0
                    ? 0
                    : Math.Round(resenas.Average(x => (decimal)x.CalificacionGeneral), 2),
                TotalResenas = resenas.Count,
                Resenas = resenas.Select(x => CrearDto(x, x.Evaluador)).ToList()
            };

            return Result<ResumenCalificacionesDto>.Ok(resumen);
        }

        public async Task<Result<EstadoCalificacionSolicitudDto>> ObtenerEstadoSolicitudAsync(
            Guid solicitudServicioId,
            Guid usuarioId,
            CancellationToken cancellationToken = default)
        {
            var solicitudes = await _unitOfWork.Repository<SolicitudServicio>()
                .GetAsync(
                    x => x.Id == solicitudServicioId && x.Activo,
                    cancellationToken: cancellationToken,
                    includes: [x => x.Cliente, x => x.Servicio, x => x.Servicio.Usuario]);

            var solicitud = solicitudes.FirstOrDefault();
            if (solicitud is null)
                return Result<EstadoCalificacionSolicitudDto>.Fail("La solicitud no existe.");

            var proveedor = solicitud.Servicio.Usuario;
            var usuarioACalificar = usuarioId switch
            {
                var id when id == solicitud.ClienteId => proveedor,
                var id when id == proveedor.Id => solicitud.Cliente,
                _ => null
            };

            if (usuarioACalificar is null)
                return Result<EstadoCalificacionSolicitudDto>.Fail("No tienes permiso para consultar esta solicitud.");

            var resenas = await _unitOfWork.Repository<Resena>()
                .GetAsync(
                    x => x.Activo &&
                         (x.EvaluadoId == usuarioACalificar.Id ||
                          (x.SolicitudServicioId == solicitud.Id && x.EvaluadorId == usuarioId)),
                    cancellationToken: cancellationToken,
                    includes: [x => x.Evaluador]);

            var miResena = resenas.FirstOrDefault(x =>
                x.SolicitudServicioId == solicitud.Id && x.EvaluadorId == usuarioId);
            var recibidas = resenas.Where(x => x.EvaluadoId == usuarioACalificar.Id).ToList();
            var completada = solicitud.Estado == EstadoSolicitudServicio.Completada;

            return Result<EstadoCalificacionSolicitudDto>.Ok(new EstadoCalificacionSolicitudDto
            {
                SolicitudServicioId = solicitud.Id,
                SolicitudCompletada = completada,
                PuedeCalificar = completada && miResena is null,
                YaCalifico = miResena is not null,
                UsuarioACalificarId = usuarioACalificar.Id,
                UsuarioACalificar = $"{usuarioACalificar.Nombre} {usuarioACalificar.Apellido}",
                CalificacionPromedioUsuario = recibidas.Count == 0
                    ? 0
                    : Math.Round(recibidas.Average(x => (decimal)x.CalificacionGeneral), 2),
                TotalResenasUsuario = recibidas.Count,
                MiResena = miResena is null ? null : CrearDto(miResena, miResena.Evaluador)
            });
        }

        private static ResenaDto CrearDto(Resena resena, Usuario evaluador) => new()
        {
            Id = resena.Id,
            SolicitudServicioId = resena.SolicitudServicioId,
            EvaluadorId = resena.EvaluadorId,
            Evaluador = $"{evaluador.Nombre} {evaluador.Apellido}",
            EvaluadoId = resena.EvaluadoId,
            CalificacionGeneral = resena.CalificacionGeneral,
            Comentario = resena.Comentario,
            FechaCreacion = resena.FechaCreacion
        };
    }
}
