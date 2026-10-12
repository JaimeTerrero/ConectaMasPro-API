using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Catalogos;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface ICatalogoService
    {
        Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerInteresesAsync(
            CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerCategoriasActividadesSocialesAsync(
            CancellationToken cancellationToken = default);
    }
}
