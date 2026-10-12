using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Application.DTOs.Pagos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService) => _pagoService = pagoService;

    [HttpGet("configuracion")]
    public IActionResult ObtenerConfiguracion()
    {
        var result = _pagoService.ObtenerConfiguracionPublica();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("solicitudes/{solicitudId:guid}/resumen")]
    public async Task<IActionResult> ObtenerResumen(Guid solicitudId, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var clienteId)) return Unauthorized("Token inválido.");
        var result = await _pagoService.ObtenerResumenAsync(solicitudId, clienteId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("solicitudes/{solicitudId:guid}/intencion")]
    public async Task<IActionResult> CrearIntencion(Guid solicitudId, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var clienteId)) return Unauthorized("Token inválido.");
        var result = await _pagoService.CrearIntencionAsync(solicitudId, clienteId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{pagoId:guid}/sincronizar")]
    public async Task<IActionResult> Sincronizar(Guid pagoId, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var clienteId)) return Unauthorized("Token inválido.");
        var result = await _pagoService.SincronizarAsync(pagoId, clienteId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("proveedor/ganancias")]
    public async Task<IActionResult> ObtenerGananciasProveedor(
        GananciasProveedorFiltroRequest filtro, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var proveedorId)) return Unauthorized("Token inválido.");
        var result = await _pagoService.ObtenerGananciasProveedorAsync(proveedorId, filtro, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("cliente/historial")]
    public async Task<IActionResult> ObtenerHistorialPagosCliente(
        HistorialPagosClienteFiltroRequest filtro, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var clienteId)) return Unauthorized("Token inválido.");
        var result = await _pagoService.ObtenerHistorialPagosClienteAsync(clienteId, filtro, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> WebhookStripe(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var contenido = await reader.ReadToEndAsync(ct);
        var firma = Request.Headers["Stripe-Signature"].ToString();
        var result = await _pagoService.ProcesarWebhookAsync(contenido, firma, ct);
        return result.Success ? Ok() : BadRequest(result);
    }

    private bool TryGetUsuarioId(out Guid usuarioId) =>
        Guid.TryParse(User.FindFirst("usuarioId")?.Value, out usuarioId);
}
