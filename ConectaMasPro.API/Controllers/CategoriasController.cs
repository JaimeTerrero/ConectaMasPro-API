using ConectaMasPro.Application.DTOs.Categorias;
using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConectaMasPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearCategoriaRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoriaService.CrearAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas(CancellationToken cancellationToken)
    {
        var result = await _categoriaService.ObtenerTodasAsync(cancellationToken);
        return Ok(result);
    }
}