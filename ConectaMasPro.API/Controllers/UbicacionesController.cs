using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Application.DTOs.Ubicaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UbicacionesController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionesController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        [HttpGet("provincias")]
        public async Task<IActionResult> ObtenerProvincias(
            CancellationToken cancellationToken)
        {
            var result = await _ubicacionService
                .ObtenerProvinciasAsync(cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("provincias/{provinciaId:guid}/municipios")]
        public async Task<IActionResult> ObtenerMunicipiosPorProvincia(
            Guid provinciaId,
            CancellationToken cancellationToken)
        {
            var result = await _ubicacionService
                .ObtenerMunicipiosPorProvinciaAsync(
                    provinciaId,
                    cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("municipios/{municipioId:guid}/sectores")]
        public async Task<IActionResult> ObtenerSectoresPorMunicipio(
            Guid municipioId,
            CancellationToken cancellationToken)
        {
            var result = await _ubicacionService
                .ObtenerSectoresPorMunicipioAsync(
                    municipioId,
                    cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("resolver-google")]
        public async Task<IActionResult> ResolverUbicacionGoogle(
            [FromBody] ResolverUbicacionGoogleRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _ubicacionService
                .ResolverUbicacionGoogleAsync(request, cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
