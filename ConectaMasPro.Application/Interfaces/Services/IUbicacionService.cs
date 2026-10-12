using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Catalogos;
using ConectaMasPro.Application.DTOs.Ubicaciones;

namespace ConectaMasPro.Application.Interfaces.Services
{
    public interface IUbicacionService
    {
        Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerProvinciasAsync(
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerMunicipiosPorProvinciaAsync(
            Guid provinciaId,
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<SectorCatalogoDto>>> ObtenerSectoresPorMunicipioAsync(
            Guid municipioId,
            CancellationToken cancellationToken = default);

        Task<Result<UbicacionCatalogoResueltaDto>> ResolverUbicacionGoogleAsync(
            ResolverUbicacionGoogleRequest request,
            CancellationToken cancellationToken = default);
    }
}
