using ConectaMasPro.API.Hubs;
using ConectaMasPro.Application.DTOs.ActividadesSociales;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ConectaMasPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActividadesSocialesController : ControllerBase
{
    private readonly IActividadSocialService _service;
    private readonly IHubContext<MensajeriaHub> _hub;

    public ActividadesSocialesController(IActividadSocialService service, IHubContext<MensajeriaHub> hub)
    {
        _service = service;
        _hub = hub;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearActividadSocialRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.CrearAsync(usuarioId, request, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("explorar")]
    public async Task<IActionResult> Explorar(ActividadSocialFiltroRequest filtro, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.ExplorarAsync(usuarioId, filtro, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerDetalle(Guid id, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.ObtenerDetalleAsync(id, usuarioId, ct);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("mis-actividades")]
    public async Task<IActionResult> ObtenerMisActividades(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        return Ok(await _service.ObtenerMisActividadesAsync(usuarioId, ct));
    }

    [HttpPost("{id:guid}/postulaciones")]
    public async Task<IActionResult> Postularse(Guid id, CrearPostulacionActividadRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.PostularseAsync(id, usuarioId, request, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}/postulaciones")]
    public async Task<IActionResult> ObtenerPostulaciones(Guid id, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.ObtenerPostulacionesAsync(id, usuarioId, ct);
        return result.Success ? Ok(result) : StatusCode(StatusCodes.Status403Forbidden, result);
    }

    [HttpGet("mis-postulaciones")]
    public async Task<IActionResult> ObtenerMisPostulaciones(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        return Ok(await _service.ObtenerMisPostulacionesAsync(usuarioId, ct));
    }

    [HttpPatch("postulaciones/{postulacionId:guid}/decision")]
    public async Task<IActionResult> Decidir(Guid postulacionId, DecidirPostulacionRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.DecidirPostulacionAsync(postulacionId, usuarioId, request, ct);
        if (!result.Success) return BadRequest(result);
        if (result.Data?.ConversacionId is not null)
            await _hub.Clients.Group(MensajeriaHub.GrupoUsuario(result.Data.PostulanteId))
                .SendAsync("ConversacionIniciada", result.Data, ct);
        return Ok(result);
    }

    [HttpPatch("postulaciones/{postulacionId:guid}/retirar")]
    public async Task<IActionResult> Retirar(Guid postulacionId, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.RetirarPostulacionAsync(postulacionId, usuarioId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, CambiarEstadoActividadRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized("Token inválido.");
        var result = await _service.CambiarEstadoAsync(id, usuarioId, request, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetUsuarioId(out Guid usuarioId) => Guid.TryParse(User.FindFirst("usuarioId")?.Value, out usuarioId);
}
