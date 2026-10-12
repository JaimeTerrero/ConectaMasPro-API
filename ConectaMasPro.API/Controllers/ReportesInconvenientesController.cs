using ConectaMasPro.Application.DTOs.ReportesInconvenientes;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/reportes-inconvenientes")]
    [Authorize]
    public class ReportesInconvenientesController : ControllerBase
    {
        private readonly IReporteInconvenienteService _reporteService;

        public ReportesInconvenientesController(IReporteInconvenienteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearReporteInconvenienteRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _reporteService.CrearAsync(usuarioId, request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:guid}/evidencia")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<IActionResult> ActualizarEvidencia(
            Guid id,
            [FromForm] ActualizarEvidenciaReporteForm form,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            if (form.Evidencia is null || form.Evidencia.Length == 0)
                return BadRequest("La evidencia es obligatoria.");

            var imagen = await ImagenFormHelper.LeerAsync(
                form.Evidencia,
                10 * 1024 * 1024,
                cancellationToken);

            if (!imagen.Success || imagen.Data is null)
                return BadRequest(imagen);

            var request = new ActualizarEvidenciaReporteRequest
            {
                Contenido = imagen.Data,
                TipoContenido = form.Evidencia.ContentType.ToLowerInvariant()
            };

            var result = await _reporteService.ActualizarEvidenciaAsync(
                id,
                usuarioId,
                request,
                cancellationToken);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("mis-reportes")]
        public async Task<IActionResult> ObtenerMisReportes(
            [FromBody] ReporteInconvenienteFiltroRequest filtro,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _reporteService.ObtenerMisReportesAsync(usuarioId, filtro, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerDetalle(Guid id, CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var esAdministrador = string.Equals(
                User.FindFirst("tipoUsuario")?.Value,
                "Administrador",
                StringComparison.OrdinalIgnoreCase);

            var result = await _reporteService.ObtenerDetalleAsync(id, usuarioId, esAdministrador, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("administracion")]
        public async Task<IActionResult> ObtenerTodos(
            [FromBody] ReporteInconvenienteFiltroRequest filtro,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _reporteService.ObtenerTodosAsync(usuarioId, filtro, cancellationToken);
            return result.Success ? Ok(result) : StatusCode(StatusCodes.Status403Forbidden, result);
        }

        [HttpPatch("{id:guid}/estado")]
        public async Task<IActionResult> ActualizarEstado(
            Guid id,
            [FromBody] ActualizarEstadoReporteRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _reporteService.ActualizarEstadoAsync(id, usuarioId, request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private bool TryObtenerUsuarioId(out Guid usuarioId) =>
            Guid.TryParse(User.FindFirst("usuarioId")?.Value, out usuarioId);
    }
}
