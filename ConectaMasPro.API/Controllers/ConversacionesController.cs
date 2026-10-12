using ConectaMasPro.API.Hubs;
using ConectaMasPro.Application.DTOs.Conversaciones;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ConectaMasPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversacionesController : ControllerBase
{
    private readonly IConversacionService _service;
    private readonly IHubContext<MensajeriaHub> _hub;

    public ConversacionesController(IConversacionService service, IHubContext<MensajeriaHub> hub)
    {
        _service = service;
        _hub = hub;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearConversacionRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();
        var result = await _service.CrearAsync(request, usuarioId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerConversaciones(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();
        return Ok(await _service.ObtenerConversacionesAsync(usuarioId, ct));
    }

    [HttpGet("{conversacionId:guid}/mensajes")]
    public async Task<IActionResult> ObtenerMensajes(Guid conversacionId, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();
        var result = await _service.ObtenerMensajesAsync(conversacionId, usuarioId, ct);
        return result.Success ? Ok(result) : StatusCode(StatusCodes.Status403Forbidden, result);
    }

    [HttpPost("mensajes")]
    public async Task<IActionResult> EnviarMensaje(EnviarMensajeRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var usuarioId)) return Unauthorized();
        var result = await _service.EnviarMensajeAsync(request, usuarioId, ct);
        if (!result.Success) return BadRequest(result);

        await _hub.Clients.Group(MensajeriaHub.GrupoConversacion(request.ConversacionId))
            .SendAsync("MensajeRecibido", result.Data, ct);
        return Ok(result);
    }

    private bool TryGetUsuarioId(out Guid usuarioId) =>
        Guid.TryParse(User.FindFirst("usuarioId")?.Value, out usuarioId);
}
