using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("cliente")]
        public async Task<IActionResult> ObtenerDashboardCliente(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var clienteId))
                return Unauthorized("Token inválido.");

            var result = await _dashboardService.ObtenerDashboardClienteAsync(
                clienteId,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("proveedor")]
        public async Task<IActionResult> ObtenerDashboardProveedor(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var proveedorId))
                return Unauthorized("Token inválido.");

            var result = await _dashboardService.ObtenerDashboardProveedorAsync(
                proveedorId,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
