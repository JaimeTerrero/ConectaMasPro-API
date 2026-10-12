using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Categorias;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface ICategoriaService
    {
        Task<Result<CategoriaDto>> CrearAsync(
        CrearCategoriaRequest request,
        CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<CategoriaDto>>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default);
    }
}
