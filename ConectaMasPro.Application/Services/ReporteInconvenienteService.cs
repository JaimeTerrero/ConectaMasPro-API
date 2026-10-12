using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Extensions;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.ReportesInconvenientes;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class ReporteInconvenienteService : IReporteInconvenienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArchivoStorage _archivoStorage;

        public ReporteInconvenienteService(IUnitOfWork unitOfWork, IArchivoStorage archivoStorage)
        {
            _unitOfWork = unitOfWork;
            _archivoStorage = archivoStorage;
        }

        public async Task<Result<ReporteInconvenienteDto>> CrearAsync(
            Guid reportanteId,
            CrearReporteInconvenienteRequest request,
            CancellationToken cancellationToken = default)
        {
            if (reportanteId == Guid.Empty)
                return Result<ReporteInconvenienteDto>.Fail("El usuario autenticado no es válido.");

            if (!Enum.IsDefined(request.Tipo))
                return Result<ReporteInconvenienteDto>.Fail("El tipo de reporte no es válido.");

            if (string.IsNullOrWhiteSpace(request.Asunto) || request.Asunto.Trim().Length > 150)
                return Result<ReporteInconvenienteDto>.Fail("El asunto es obligatorio y no puede exceder 150 caracteres.");

            if (string.IsNullOrWhiteSpace(request.Descripcion) || request.Descripcion.Trim().Length > 3000)
                return Result<ReporteInconvenienteDto>.Fail("La descripción es obligatoria y no puede exceder 3000 caracteres.");

            var reportante = await _unitOfWork.Repository<Usuario>().GetByIdAsync(reportanteId, cancellationToken);
            if (reportante is null || !reportante.Activo)
                return Result<ReporteInconvenienteDto>.Fail("El usuario autenticado no existe.");

            SolicitudServicio? solicitud = null;
            Guid? reportadoId = null;

            if (request.Tipo == TipoReporteInconveniente.Aplicacion)
            {
                if (request.SolicitudServicioId.HasValue)
                    return Result<ReporteInconvenienteDto>.Fail("Un reporte de la aplicación no debe estar asociado a una solicitud de servicio.");
            }
            else
            {
                if (!request.SolicitudServicioId.HasValue || request.SolicitudServicioId == Guid.Empty)
                    return Result<ReporteInconvenienteDto>.Fail("La solicitud de servicio es obligatoria para este tipo de reporte.");

                var solicitudes = await _unitOfWork.Repository<SolicitudServicio>().GetWithQueryAsync(
                    predicate: x => x.Id == request.SolicitudServicioId && x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes: [x => x.Cliente, x => x.Servicio, x => x.Servicio.Usuario]);

                solicitud = solicitudes.FirstOrDefault();
                if (solicitud is null)
                    return Result<ReporteInconvenienteDto>.Fail("La solicitud de servicio no existe.");

                var proveedorId = solicitud.Servicio.UsuarioId;

                if (request.Tipo == TipoReporteInconveniente.ServicioContratado)
                {
                    if (solicitud.ClienteId != reportanteId)
                        return Result<ReporteInconvenienteDto>.Fail("Solo el cliente de la solicitud puede reportar un inconveniente con el servicio.");

                    reportadoId = proveedorId;
                }
                else
                {
                    if (proveedorId != reportanteId)
                        return Result<ReporteInconvenienteDto>.Fail("Solo el proveedor de la solicitud puede reportar un inconveniente con el cliente.");

                    reportadoId = solicitud.ClienteId;
                }
            }

            var reporte = new ReporteInconveniente
            {
                ReportanteId = reportanteId,
                ReportadoId = reportadoId,
                SolicitudServicioId = solicitud?.Id,
                Tipo = request.Tipo,
                Estado = EstadoReporteInconveniente.Abierto,
                Asunto = request.Asunto.Trim(),
                Descripcion = request.Descripcion.Trim()
            };

            await _unitOfWork.Repository<ReporteInconveniente>().AddAsync(reporte, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reporteCreado = (await CargarReporteAsync(reporte.Id, cancellationToken)).First();

            return Result<ReporteInconvenienteDto>.Ok(Mapear(reporteCreado), "El inconveniente fue reportado correctamente.");
        }

        public async Task<Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>> ObtenerMisReportesAsync(
            Guid reportanteId,
            ReporteInconvenienteFiltroRequest filtro,
            CancellationToken cancellationToken = default)
        {
            if (reportanteId == Guid.Empty)
                return Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>.Fail("El usuario autenticado no es válido.");

            var reportes = await ConsultarAsync(filtro, x => x.ReportanteId == reportanteId && x.Activo, cancellationToken);
            return Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>.Ok(Paginar(reportes, filtro));
        }

        public async Task<Result<ReporteInconvenienteDto>> ObtenerDetalleAsync(
            Guid reporteId,
            Guid usuarioId,
            bool esAdministrador,
            CancellationToken cancellationToken = default)
        {
            var reportes = await CargarReporteAsync(reporteId, cancellationToken);
            var reporte = reportes.FirstOrDefault();

            if (reporte is null)
                return Result<ReporteInconvenienteDto>.Fail("El reporte no existe.");

            if (reporte.ReportanteId != usuarioId)
            {
                if (!esAdministrador || !await EsAdministradorAsync(usuarioId, cancellationToken))
                    return Result<ReporteInconvenienteDto>.Fail("No tienes permiso para consultar este reporte.");
            }

            return Result<ReporteInconvenienteDto>.Ok(Mapear(reporte));
        }

        public async Task<Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>> ObtenerTodosAsync(
            Guid administradorId,
            ReporteInconvenienteFiltroRequest filtro,
            CancellationToken cancellationToken = default)
        {
            if (!await EsAdministradorAsync(administradorId, cancellationToken))
                return Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>.Fail("No tienes permiso para consultar todos los reportes.");

            var reportes = await ConsultarAsync(filtro, x => x.Activo, cancellationToken);
            return Result<PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>>.Ok(Paginar(reportes, filtro));
        }

        public async Task<Result<ReporteInconvenienteDto>> ActualizarEstadoAsync(
            Guid reporteId,
            Guid administradorId,
            ActualizarEstadoReporteRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!await EsAdministradorAsync(administradorId, cancellationToken))
                return Result<ReporteInconvenienteDto>.Fail("No tienes permiso para administrar reportes.");

            if (!Enum.IsDefined(request.Estado))
                return Result<ReporteInconvenienteDto>.Fail("El estado del reporte no es válido.");

            if (!string.IsNullOrWhiteSpace(request.Resolucion) && request.Resolucion.Trim().Length > 2000)
                return Result<ReporteInconvenienteDto>.Fail("La resolución no puede exceder 2000 caracteres.");

            if (request.Estado == EstadoReporteInconveniente.Resuelto && string.IsNullOrWhiteSpace(request.Resolucion))
                return Result<ReporteInconvenienteDto>.Fail("Debes indicar la resolución para cerrar el reporte como resuelto.");

            var reportes = await CargarReporteAsync(reporteId, cancellationToken);
            var reporte = reportes.FirstOrDefault();
            if (reporte is null)
                return Result<ReporteInconvenienteDto>.Fail("El reporte no existe.");

            reporte.Estado = request.Estado;
            reporte.Resolucion = string.IsNullOrWhiteSpace(request.Resolucion) ? null : request.Resolucion.Trim();
            reporte.FechaResolucion = request.Estado is EstadoReporteInconveniente.Resuelto or EstadoReporteInconveniente.Rechazado
                ? DateTime.UtcNow
                : null;
            reporte.FechaModificacion = DateTime.UtcNow;

            _unitOfWork.Repository<ReporteInconveniente>().Update(reporte);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ReporteInconvenienteDto>.Ok(Mapear(reporte), "El estado del reporte fue actualizado correctamente.");
        }

        public async Task<Result<EvidenciaReporteDto>> ActualizarEvidenciaAsync(
            Guid reporteId,
            Guid reportanteId,
            ActualizarEvidenciaReporteRequest request,
            CancellationToken cancellationToken = default)
        {
            if (reporteId == Guid.Empty || reportanteId == Guid.Empty)
                return Result<EvidenciaReporteDto>.Fail("El reporte o el usuario no es válido.");

            if (request.Contenido.Length == 0)
                return Result<EvidenciaReporteDto>.Fail("La evidencia es obligatoria.");

            if (request.Contenido.Length > 10 * 1024 * 1024)
                return Result<EvidenciaReporteDto>.Fail("La evidencia no puede superar 10 MB.");

            string[] tiposPermitidos = ["image/jpeg", "image/png", "image/webp"];
            if (!tiposPermitidos.Contains(request.TipoContenido, StringComparer.OrdinalIgnoreCase))
                return Result<EvidenciaReporteDto>.Fail("El tipo de imagen no está permitido.");

            var reporte = (await CargarReporteAsync(reporteId, cancellationToken)).FirstOrDefault();
            if (reporte is null)
                return Result<EvidenciaReporteDto>.Fail("El reporte no existe.");

            if (reporte.ReportanteId != reportanteId)
                return Result<EvidenciaReporteDto>.Fail("Solo quien creó el reporte puede cargar su evidencia.");

            var evidenciaAnterior = reporte.EvidenciaUrl;
            var nuevaEvidenciaUrl = await _archivoStorage.GuardarImagenAsync(
                request.Contenido,
                request.TipoContenido,
                "evidencias",
                cancellationToken);

            reporte.EvidenciaUrl = nuevaEvidenciaUrl;
            reporte.FechaModificacion = DateTime.UtcNow;
            _unitOfWork.Repository<ReporteInconveniente>().Update(reporte);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _archivoStorage.EliminarAsync(nuevaEvidenciaUrl, CancellationToken.None);
                throw;
            }

            await _archivoStorage.EliminarAsync(evidenciaAnterior, cancellationToken);

            return Result<EvidenciaReporteDto>.Ok(
                new EvidenciaReporteDto
                {
                    ReporteId = reporte.Id,
                    EvidenciaUrl = nuevaEvidenciaUrl
                },
                "La evidencia del reporte fue actualizada correctamente.");
        }

        private async Task<IReadOnlyList<ReporteInconveniente>> ConsultarAsync(
            ReporteInconvenienteFiltroRequest filtro,
            System.Linq.Expressions.Expression<Func<ReporteInconveniente, bool>> predicate,
            CancellationToken cancellationToken)
        {
            var busqueda = filtro.Busqueda?.Trim().ToLower();
            return await _unitOfWork.Repository<ReporteInconveniente>().GetWithQueryAsync(
                predicate: predicate,
                orderBy: null,
                queryModifier: query => query
                    .WithCondition(x => x.Tipo == filtro.Tipo, filtro.Tipo.HasValue)
                    .WithCondition(x => x.Estado == filtro.Estado, filtro.Estado.HasValue)
                    .WithCondition(x => x.Asunto.ToLower().Contains(busqueda!) || x.Descripcion.ToLower().Contains(busqueda!), !string.IsNullOrWhiteSpace(busqueda))
                    .OrderByDescending(x => x.FechaCreacion),
                cancellationToken: cancellationToken,
                includes: [x => x.Reportante, x => x.Reportado!, x => x.SolicitudServicio!, x => x.SolicitudServicio!.Servicio]);
        }

        private Task<IReadOnlyList<ReporteInconveniente>> CargarReporteAsync(Guid reporteId, CancellationToken cancellationToken) =>
            _unitOfWork.Repository<ReporteInconveniente>().GetWithQueryAsync(
                predicate: x => x.Id == reporteId && x.Activo,
                cancellationToken: cancellationToken,
                includes: [x => x.Reportante, x => x.Reportado!, x => x.SolicitudServicio!, x => x.SolicitudServicio!.Servicio]);

        private async Task<bool> EsAdministradorAsync(Guid usuarioId, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId, cancellationToken);
            return usuario is not null && usuario.Activo && usuario.TipoUsuario == TipoUsuario.Administrador;
        }

        private static PagedResponse<IReadOnlyList<ReporteInconvenienteDto>> Paginar(
            IReadOnlyList<ReporteInconveniente> reportes,
            ReporteInconvenienteFiltroRequest filtro)
        {
            var pagina = reportes.Skip((filtro.Page - 1) * filtro.PageSize).Take(filtro.PageSize).Select(Mapear).ToList();
            return PagedResponse<IReadOnlyList<ReporteInconvenienteDto>>.Create(pagina, filtro.Page, filtro.PageSize, reportes.Count);
        }

        private static ReporteInconvenienteDto Mapear(ReporteInconveniente reporte) => new()
        {
            Id = reporte.Id,
            ReportanteId = reporte.ReportanteId,
            Reportante = $"{reporte.Reportante.Nombre} {reporte.Reportante.Apellido}",
            ReportadoId = reporte.ReportadoId,
            Reportado = reporte.Reportado is null ? null : $"{reporte.Reportado.Nombre} {reporte.Reportado.Apellido}",
            SolicitudServicioId = reporte.SolicitudServicioId,
            Servicio = reporte.SolicitudServicio?.Servicio?.Titulo,
            TipoId = (int)reporte.Tipo,
            Tipo = reporte.Tipo.ToString(),
            EstadoId = (int)reporte.Estado,
            Estado = reporte.Estado.ToString(),
            Asunto = reporte.Asunto,
            Descripcion = reporte.Descripcion,
            EvidenciaUrl = reporte.EvidenciaUrl,
            Resolucion = reporte.Resolucion,
            FechaCreacion = reporte.FechaCreacion,
            FechaResolucion = reporte.FechaResolucion
        };
    }
}
