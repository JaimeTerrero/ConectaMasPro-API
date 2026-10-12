using ConectaMasPro.Application.DTOs.Resenas;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResenasController : ControllerBase
    {
        private readonly IResenaService _resenaService;

        public ResenasController(IResenaService resenaService)
        {
            _resenaService = resenaService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearResenaRequest request, CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _resenaService.CrearAsync(usuarioId, request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("usuario/{usuarioId:guid}")]
        public async Task<IActionResult> ObtenerPorUsuario(
            Guid usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _resenaService.ObtenerPorUsuarioAsync(usuarioId, cancellationToken);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        [HttpGet("solicitud/{solicitudServicioId:guid}/estado")]
        public async Task<IActionResult> ObtenerEstadoSolicitud(
            Guid solicitudServicioId,
            CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _resenaService.ObtenerEstadoSolicitudAsync(
                solicitudServicioId,
                usuarioId,
                cancellationToken);

            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        private bool TryObtenerUsuarioId(out Guid usuarioId)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;
            return Guid.TryParse(usuarioIdClaim, out usuarioId);
        }
    }
}
