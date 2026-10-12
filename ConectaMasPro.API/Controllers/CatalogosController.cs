using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet("intereses")]
        public async Task<IActionResult> ObtenerIntereses(
            CancellationToken cancellationToken)
        {
            var result = await _catalogoService
                .ObtenerInteresesAsync(cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("categorias-actividades-sociales")]
        public async Task<IActionResult> ObtenerCategoriasActividadesSociales(
            CancellationToken cancellationToken)
        {
            var result = await _catalogoService.ObtenerCategoriasActividadesSocialesAsync(cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
