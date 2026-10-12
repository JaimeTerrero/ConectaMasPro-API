using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Catalogos;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerInteresesAsync(
            CancellationToken cancellationToken = default)
        {
            var intereses = await _unitOfWork
                .Repository<Interes>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query.OrderBy(x => x.Nombre),
                    includes: []);

            var response = intereses
                .Select(x => new CatalogoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToList();

            return Result<IReadOnlyList<CatalogoDto>>.Ok(response);
        }

        public async Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerCategoriasActividadesSocialesAsync(
            CancellationToken cancellationToken = default)
        {
            var categorias = await _unitOfWork.Repository<CategoriaActividadSocial>().GetWithQueryAsync(
                x => x.Activo,
                null,
                query => query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre),
                cancellationToken);
            return Result<IReadOnlyList<CatalogoDto>>.Ok(categorias.Select(x => new CatalogoDto
            {
                Id = x.Id,
                Nombre = x.Nombre
            }).ToList());
        }
    }
}
