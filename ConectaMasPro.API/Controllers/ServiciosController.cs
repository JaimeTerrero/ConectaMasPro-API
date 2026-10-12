using ConectaMasPro.Application.DTOs.Servicios;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet("{id:guid}/detalle")]
        public async Task<IActionResult> ObtenerDetalle(Guid id, CancellationToken cancellationToken)
        {
            var result = await _servicioService.ObtenerDetalleAsync(id, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("explorar")]
        public async Task<IActionResult> ExplorarServicios([FromBody] ServicioFiltroRequest filtro, CancellationToken cancellationToken)
        {
            var result = await _servicioService.ExplorarServiciosAsync(
                filtro,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPost("mis-servicios")]
        public async Task<IActionResult> ObtenerMisServicios([FromBody] ServicioFiltroRequest filtro,
            CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _servicioService.ObtenerMisServiciosAsync(
                usuarioId,
                filtro,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearServicioRequest request, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _servicioService.CrearAsync(
                usuarioId,
                request,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerActivos(CancellationToken cancellationToken)
        {
            var result = await _servicioService.ObtenerActivosAsync(cancellationToken);
            return Ok(result.Data);
        }
    }
}
