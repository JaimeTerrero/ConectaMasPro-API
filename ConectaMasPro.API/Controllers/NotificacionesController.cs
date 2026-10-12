using ConectaMasPro.Application.DTOs.Notificaciones;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearNotificacionRequest request, CancellationToken cancellationToken)
        {
            var result = await _notificacionService.CrearAsync(request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpPatch("{id:guid}/marcar-como-leida")]
        public async Task<IActionResult> MarcarComoLeida(Guid id, CancellationToken cancellationToken)
        {
            var result = await _notificacionService.MarcarComoLeidaAsync(id, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return NoContent();
        }
    }
}
