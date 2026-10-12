using ConectaMasPro.Application.DTOs.Usuarios;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearUsuarioRequest request, CancellationToken cancellationToken)
        {
            var result = await _usuarioService.CrearAsync(request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken cancellationToken)
        {
            var result = await _usuarioService.ObtenerPorIdAsync(id, cancellationToken);

            if (!result.Success)
                return NotFound(result);

            return Ok(result.Data);
        }
    }
}
