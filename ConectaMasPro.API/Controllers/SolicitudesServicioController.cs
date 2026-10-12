using ConectaMasPro.Application.DTOs.Solicitudes;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.API.Hubs;
using ConectaMasPro.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitudesServicioController : ControllerBase
    {
        private readonly ISolicitudServicioService _solicitudServicioService;
        private readonly IHubContext<MensajeriaHub> _mensajeriaHub;

        public SolicitudesServicioController(
            ISolicitudServicioService solicitudServicioService,
            IHubContext<MensajeriaHub> mensajeriaHub)
        {
            _solicitudServicioService = solicitudServicioService;
            _mensajeriaHub = mensajeriaHub;
        }

        [HttpPost("mis-solicitudes")]
        public async Task<IActionResult> ObtenerMisSolicitudes([FromBody] SolicitudServicioFiltroRequest filtro,
            CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _solicitudServicioService.ObtenerMisSolicitudesAsync(
                usuarioId,
                filtro,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPost("solicitudes-recibidas")]
        public async Task<IActionResult> ObtenerSolicitudesRecibidas([FromBody] SolicitudServicioFiltroRequest filtro, CancellationToken cancellationToken)
        {
            var usuarioIdClaim =
                User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            {
                return Unauthorized(
                    "Token inválido.");
            }

            if (!Guid.TryParse(
                usuarioIdClaim,
                out var proveedorId))
            {
                return Unauthorized(
                    "Token inválido.");
            }

            var result = await _solicitudServicioService.ObtenerSolicitudesRecibidasAsync(proveedorId, filtro, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(
                    result.Message);
            }

            return Ok(
                result.Data);
        }

        [HttpGet("{id:guid}/detalle")]
        public async Task<IActionResult> ObtenerDetalle(Guid id, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _solicitudServicioService.ObtenerDetalleAsync(
                id,
                usuarioId,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearSolicitudServicioRequest request, CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var clienteId))
                return Unauthorized("Token inválido.");

            var result = await _solicitudServicioService.CrearAsync(clienteId, request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/estado")]
        public async Task<IActionResult> CambiarEstado(Guid id, EstadoSolicitudServicio estado, CancellationToken cancellationToken)
        {
            var usuarioIdClaim =
                User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            {
                return Unauthorized(
                    "Token inválido.");
            }

            if (!Guid.TryParse(
                usuarioIdClaim,
                out var proveedorId))
            {
                return Unauthorized(
                    "Token inválido.");
            }

            var result = await 
                    _solicitudServicioService.CambiarEstadoAsync(
                        id,
                        proveedorId,
                        estado,
                        cancellationToken
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            if (result.Data?.ConversacionId is not null)
            {
                await _mensajeriaHub.Clients
                    .Group(MensajeriaHub.GrupoUsuario(result.Data.ClienteId))
                    .SendAsync("ConversacionIniciada", result.Data, cancellationToken);
            }

            if (result.Data is not null && estado == EstadoSolicitudServicio.PendienteConfirmacionCliente)
            {
                await _mensajeriaHub.Clients
                    .Group(MensajeriaHub.GrupoUsuario(result.Data.ClienteId))
                    .SendAsync("FinalizacionServicioReportada", result.Data, cancellationToken);
            }

            return Ok(result);
        }

        [HttpPatch("{id:guid}/precio-acordado")]
        public async Task<IActionResult> EstablecerPrecioAcordado(Guid id,
            EstablecerPrecioAcordadoRequest request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst("usuarioId")?.Value, out var proveedorId))
                return Unauthorized("Token inválido.");
            var result = await _solicitudServicioService.EstablecerPrecioAcordadoAsync(
                id, proveedorId, request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:guid}/confirmacion-finalizacion")]
        public async Task<IActionResult> ConfirmarFinalizacion(Guid id,
            ConfirmarFinalizacionRequest request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst("usuarioId")?.Value, out var clienteId))
                return Unauthorized("Token inválido.");
            var result = await _solicitudServicioService.ConfirmarFinalizacionAsync(
                id, clienteId, request, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
