using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Categorias;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoriaDto>> CrearAsync(
    CrearCategoriaRequest request,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return Result<CategoriaDto>.Fail("El nombre es obligatorio.");

            var existe = await _unitOfWork
                .Repository<CategoriaServicio>()
                .FindAsync(
                    x => x.Nombre.ToLower() == request.Nombre.Trim().ToLower() && x.Activo,
                    cancellationToken);

            if (existe.Any())
                return Result<CategoriaDto>.Fail("Ya existe una categoría con ese nombre.");

            var categoria = new CategoriaServicio
            {
                Nombre = request.Nombre.Trim()
            };

            await _unitOfWork
                .Repository<CategoriaServicio>()
                .AddAsync(categoria, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            return Result<CategoriaDto>.Ok(dto);
        }

        public async Task<Result<IReadOnlyList<CategoriaDto>>> ObtenerTodasAsync(
            CancellationToken cancellationToken = default)
        {
            var categorias = await _unitOfWork
                .Repository<CategoriaServicio>()
                .FindAsync(x => x.Activo, cancellationToken);

            var result = categorias
                .Select(x => new CategoriaDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion
                })
                .ToList();

            return Result<IReadOnlyList<CategoriaDto>>.Ok(result);
        }
    }
}
