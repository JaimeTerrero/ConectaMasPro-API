using ConectaMasPro.Application.DTOs.DocumentosLegales;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/documentos-legales")]
    public class DocumentosLegalesController : ControllerBase
    {
        private readonly IDocumentoLegalService _documentoLegalService;

        public DocumentosLegalesController(IDocumentoLegalService documentoLegalService)
        {
            _documentoLegalService = documentoLegalService;
        }

        [AllowAnonymous]
        [HttpGet("vigentes")]
        public async Task<IActionResult> ObtenerVigentes(CancellationToken cancellationToken)
        {
            var result = await _documentoLegalService.ObtenerVigentesAsync(cancellationToken);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet("mis-aceptaciones")]
        public async Task<IActionResult> ObtenerMisAceptaciones(CancellationToken cancellationToken)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized("Token inválido.");

            var result = await _documentoLegalService.ObtenerMisAceptacionesAsync(
                usuarioId,
                cancellationToken);

            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        private bool TryObtenerUsuarioId(out Guid usuarioId) =>
            Guid.TryParse(User.FindFirst("usuarioId")?.Value, out usuarioId);
    }
}
