using ConectaMasPro.Application.DTOs.Conversaciones;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ConectaMasPro.API.Hubs;

[Authorize]
public class MensajeriaHub : Hub
{
    private readonly IConversacionService _service;
    public MensajeriaHub(IConversacionService service) => _service = service;

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoUsuario(ObtenerUsuarioId()));
        await base.OnConnectedAsync();
    }

    public async Task UnirseConversacion(Guid conversacionId)
    {
        var usuarioId = ObtenerUsuarioId();
        if (!await _service.EsParticipanteAsync(conversacionId, usuarioId, Context.ConnectionAborted))
            throw new HubException("No perteneces a esta conversación.");
        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoConversacion(conversacionId));
    }

    public Task SalirConversacion(Guid conversacionId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GrupoConversacion(conversacionId));

    public async Task EnviarMensaje(EnviarMensajeRequest request)
    {
        var result = await _service.EnviarMensajeAsync(request, ObtenerUsuarioId(), Context.ConnectionAborted);
        if (!result.Success)
            throw new HubException(result.Message ?? "No fue posible enviar el mensaje.");
        await Clients.Group(GrupoConversacion(request.ConversacionId))
            .SendAsync("MensajeRecibido", result.Data, Context.ConnectionAborted);
    }

    public static string GrupoConversacion(Guid id) => $"conversacion:{id}";
    public static string GrupoUsuario(Guid id) => $"usuario:{id}";

    private Guid ObtenerUsuarioId()
    {
        if (!Guid.TryParse(Context.User?.FindFirst("usuarioId")?.Value, out var id))
            throw new HubException("El token no contiene un usuario válido.");
        return id;
    }
}
