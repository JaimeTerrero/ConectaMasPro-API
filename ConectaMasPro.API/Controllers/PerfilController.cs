using ConectaMasPro.Application.DTOs.Perfiles;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfilController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        [HttpGet("cliente")]
        public async Task<IActionResult> ObtenerPerfilCliente(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _perfilService.ObtenerPerfilClienteAsync(
                usuarioId,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("proveedor")]
        public async Task<IActionResult> ObtenerPerfilProveedor(CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _perfilService.ObtenerPerfilProveedorAsync(
                usuarioId,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("cliente/{usuarioId:guid}")]
        public async Task<IActionResult> ObtenerPerfilPublicoCliente(
            Guid usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _perfilService.ObtenerPerfilPublicoClienteAsync(
                usuarioId,
                cancellationToken);

            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        [AllowAnonymous]
        [HttpGet("proveedor/{usuarioId:guid}")]
        public async Task<IActionResult> ObtenerPerfilPublicoProveedor(
            Guid usuarioId,
            CancellationToken cancellationToken)
        {
            var result = await _perfilService.ObtenerPerfilPublicoProveedorAsync(
                usuarioId,
                cancellationToken);

            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        [HttpPut("me")]
        public async Task<IActionResult> ActualizarMiPerfil(
            [FromBody] ActualizarPerfilRequest request,
            CancellationToken cancellationToken)
        {
            var usuarioIdClaim = User.FindFirst("usuarioId")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
                return Unauthorized("Token inválido.");

            if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _perfilService.ActualizarMiPerfilAsync(
                usuarioId,
                request,
                cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("me/foto")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> ActualizarFotoPerfil(
            [FromForm] ActualizarFotoPerfilForm form,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst("usuarioId")?.Value, out var usuarioId))
                return Unauthorized("Token inválido.");

            if (form.FotoPerfil is null || form.FotoPerfil.Length == 0)
                return BadRequest("La foto de perfil es obligatoria.");

            var imagen = await ImagenFormHelper.LeerAsync(
                form.FotoPerfil,
                5 * 1024 * 1024,
                cancellationToken);

            if (!imagen.Success || imagen.Data is null)
                return BadRequest(imagen);

            var request = new ActualizarFotoPerfilRequest
            {
                Contenido = imagen.Data,
                TipoContenido = form.FotoPerfil.ContentType.ToLowerInvariant(),
                NombreArchivo = ImagenFormHelper.NormalizarNombre(form.FotoPerfil)
            };

            var result = await _perfilService.ActualizarFotoPerfilAsync(
                usuarioId,
                request,
                cancellationToken);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
