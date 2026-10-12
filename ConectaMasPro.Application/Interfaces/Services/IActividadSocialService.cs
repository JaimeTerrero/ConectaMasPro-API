using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.ActividadesSociales;

namespace ConectaMasPro.Application.Interfaces.Services;

public interface IActividadSocialService
{
    Task<Result<Guid>> CrearAsync(Guid usuarioId, CrearActividadSocialRequest request, CancellationToken ct = default);
    Task<Result<PagedResponse<IReadOnlyList<ActividadSocialDto>>>> ExplorarAsync(Guid usuarioId, ActividadSocialFiltroRequest filtro, CancellationToken ct = default);
    Task<Result<ActividadSocialDto>> ObtenerDetalleAsync(Guid actividadId, Guid usuarioId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ActividadSocialDto>>> ObtenerMisActividadesAsync(Guid usuarioId, CancellationToken ct = default);
    Task<Result<Guid>> PostularseAsync(Guid actividadId, Guid usuarioId, CrearPostulacionActividadRequest request, CancellationToken ct = default);
    Task<Result<IReadOnlyList<PostulacionActividadDto>>> ObtenerPostulacionesAsync(Guid actividadId, Guid usuarioId, CancellationToken ct = default);
    Task<Result<IReadOnlyList<PostulacionActividadDto>>> ObtenerMisPostulacionesAsync(Guid usuarioId, CancellationToken ct = default);
    Task<Result<DecisionPostulacionDto>> DecidirPostulacionAsync(Guid postulacionId, Guid usuarioId, DecidirPostulacionRequest request, CancellationToken ct = default);
    Task<Result> RetirarPostulacionAsync(Guid postulacionId, Guid usuarioId, CancellationToken ct = default);
    Task<Result> CambiarEstadoAsync(Guid actividadId, Guid usuarioId, CambiarEstadoActividadRequest request, CancellationToken ct = default);
}
