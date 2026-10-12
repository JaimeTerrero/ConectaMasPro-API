using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Extensions;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.Servicios;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;

namespace ConectaMasPro.Application.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServicioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ServicioListadoDto>> CrearAsync(
    Guid usuarioId,
    CrearServicioRequest request,
    CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
                return Result<ServicioListadoDto>.Fail("El usuario autenticado no es válido.");

            if (request.CategoriaServicioId == Guid.Empty)
                return Result<ServicioListadoDto>.Fail("La categoría es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.Titulo))
                return Result<ServicioListadoDto>.Fail("El título del servicio es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Descripcion))
                return Result<ServicioListadoDto>.Fail("La descripción del servicio es obligatoria.");

            if (request.PrecioBase is not null && request.PrecioBase < 0)
                return Result<ServicioListadoDto>.Fail("El precio base no puede ser negativo.");

            var usuarios = await _unitOfWork
                .Repository<Usuario>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == usuarioId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: null,
                    includes:
                    [
                        x => x.Perfil!,
                        x => x.Ubicacion!,
                        x => x.Ubicacion!.Provincia,
                        x => x.Ubicacion!.Municipio,
                        x => x.Ubicacion!.Sector!
                    ]);

            var usuario = usuarios.FirstOrDefault();

            if (usuario is null || !usuario.Activo)
                return Result<ServicioListadoDto>.Fail("El usuario no existe o está inactivo.");

            var categoria = await _unitOfWork
                .Repository<CategoriaServicio>()
                .GetByIdAsync(request.CategoriaServicioId, cancellationToken);

            if (categoria is null || !categoria.Activo)
                return Result<ServicioListadoDto>.Fail("La categoría no existe o está inactiva.");

            var servicio = new Servicio
            {
                UsuarioId = usuarioId,
                CategoriaServicioId = request.CategoriaServicioId,
                Titulo = request.Titulo.Trim(),
                Descripcion = request.Descripcion.Trim(),
                PrecioBase = request.PrecioBase,
                PrecioNegociable = request.PrecioNegociable
            };

            await _unitOfWork.Repository<Servicio>()
                .AddAsync(servicio, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ServicioListadoDto
            {
                Id = servicio.Id,
                TituloServicio = servicio.Titulo,
                Descripcion = servicio.Descripcion,
                Categoria = categoria.Nombre,
                Proveedor = $"{usuario.Nombre} {usuario.Apellido}",
                CalificacionProveedor = usuario.Perfil?.CalificacionPromedio ?? 0,
                TotalResenasProveedor = usuario.Perfil?.TotalResenas ?? 0,
                ProvinciaId = usuario.Ubicacion?.ProvinciaId,
                Provincia = usuario.Ubicacion?.Provincia?.Nombre,
                UbicacionProveedor = ConstruirUbicacionProveedor(usuario.Ubicacion),
                Precio = servicio.PrecioBase,
                PrecioNegociable = servicio.PrecioNegociable,
                FechaCreacion = servicio.FechaCreacion
            };

            return Result<ServicioListadoDto>.Ok(dto);
        }

        public async Task<Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>> ExplorarServiciosAsync(
    ServicioFiltroRequest filtro,
    CancellationToken cancellationToken = default)
        {
            if (filtro.CalificacionMinima is < 0 or > 5)
                return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Fail(
                    "La calificación mínima debe estar entre 0 y 5.");

            if (filtro.ProvinciaId == Guid.Empty)
                return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Fail(
                    "La provincia no es válida.");

            var texto = filtro.Busqueda?.Trim().ToLower();
            var categoria = filtro.Categoria?.Trim().ToLower();
            var ordenarPor = filtro.OrdenarPor?.Trim().ToLower();

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .WithCondition(
                            x => x.Titulo.ToLower().Contains(texto!) ||
                                 x.Descripcion.ToLower().Contains(texto!) ||
                                 x.Usuario.Nombre.ToLower().Contains(texto!) ||
                                 x.Usuario.Apellido.ToLower().Contains(texto!) ||
                                 x.CategoriaServicio.Nombre.ToLower().Contains(texto!),
                            !string.IsNullOrWhiteSpace(texto))

                        .WithCondition(
                            x => x.CategoriaServicio.Nombre.ToLower() == categoria,
                            !string.IsNullOrWhiteSpace(categoria))

                        .WithCondition(
                            x => x.PrecioBase != null && x.PrecioBase <= filtro.PrecioMaximo,
                            filtro.PrecioMaximo is not null)

                        .WithCondition(
                            x => x.Usuario.Perfil != null &&
                                 x.Usuario.Perfil.CalificacionPromedio >= filtro.CalificacionMinima,
                            filtro.CalificacionMinima is not null)

                        .WithCondition(
                            x => x.Usuario.Ubicacion != null &&
                                 x.Usuario.Ubicacion.ProvinciaId == filtro.ProvinciaId,
                            filtro.ProvinciaId is not null)

                        .WithOrderBy(
                            x => x.PrecioBase,
                            ordenarPor == "precio_menor",
                            "asc")

                        .WithOrderBy(
                            x => x.PrecioBase,
                            ordenarPor == "precio_mayor",
                            "desc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "antiguos",
                            "asc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "recientes" || string.IsNullOrWhiteSpace(ordenarPor),
                            "desc"),
                    includes:
                    [
                        x => x.Usuario,
                        x => x.Usuario.Perfil!,
                        x => x.Usuario.Ubicacion!,
                        x => x.Usuario.Ubicacion!.Provincia,
                        x => x.Usuario.Ubicacion!.Municipio,
                        x => x.Usuario.Ubicacion!.Sector!,
                        x => x.CategoriaServicio
                    ]);

            var totalRecords = servicios.Count;

            var serviciosPaginados = servicios
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(x => new ServicioListadoDto
                {
                    Id = x.Id,
                    TituloServicio = x.Titulo,
                    Descripcion = x.Descripcion,
                    Categoria = x.CategoriaServicio.Nombre,
                    Proveedor = $"{x.Usuario.Nombre} {x.Usuario.Apellido}",
                    CalificacionProveedor = x.Usuario.Perfil?.CalificacionPromedio ?? 0,
                    TotalResenasProveedor = x.Usuario.Perfil?.TotalResenas ?? 0,
                    ProvinciaId = x.Usuario.Ubicacion?.ProvinciaId,
                    Provincia = x.Usuario.Ubicacion?.Provincia?.Nombre,
                    UbicacionProveedor = ConstruirUbicacionProveedor(x.Usuario.Ubicacion),
                    Precio = x.PrecioBase,
                    PrecioNegociable = x.PrecioNegociable,
                    FechaCreacion = x.FechaCreacion
                })
                .ToList();

            var pagedResponse = PagedResponse<IReadOnlyList<ServicioListadoDto>>.Create(
                serviciosPaginados,
                filtro.Page,
                filtro.PageSize,
                totalRecords);

            return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Ok(pagedResponse);
        }

        public async Task<Result<IReadOnlyList<ServicioDto>>> ObtenerActivosAsync(
            CancellationToken cancellationToken = default)
        {
            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .FindAsync(x => x.Activo, cancellationToken);

            var result = servicios
                .Select(x => new ServicioDto
                {
                    Id = x.Id,
                    UsuarioId = x.UsuarioId,
                    CategoriaServicioId = x.CategoriaServicioId,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    PrecioBase = x.PrecioBase,
                    PrecioNegociable = x.PrecioNegociable
                })
                .ToList();

            return Result<IReadOnlyList<ServicioDto>>.Ok(result);
        }

        public async Task<Result<ServicioDetalleDto>> ObtenerDetalleAsync(Guid servicioId, CancellationToken cancellationToken = default)
        {
            if (servicioId == Guid.Empty)
                return Result<ServicioDetalleDto>.Fail("El servicio es obligatorio.");

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == servicioId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: null,
                    includes:
                    [
                        x => x.Usuario,
                        x => x.Usuario.Perfil!,
                        x => x.Usuario.Ubicacion!,
                        x => x.Usuario.Ubicacion!.Provincia,
                        x => x.Usuario.Ubicacion!.Municipio,
                        x => x.Usuario.Ubicacion!.Sector!,
                        x => x.CategoriaServicio
                    ]);

            var servicio = servicios.FirstOrDefault();

            if (servicio is null)
                return Result<ServicioDetalleDto>.Fail("Servicio no encontrado.");

            var dto = new ServicioDetalleDto
            {
                Id = servicio.Id,
                TituloServicio = servicio.Titulo,
                Descripcion = servicio.Descripcion,
                CategoriaId = servicio.CategoriaServicioId,
                Categoria = servicio.CategoriaServicio.Nombre,
                ProveedorId = servicio.UsuarioId,
                Proveedor = $"{servicio.Usuario.Nombre} {servicio.Usuario.Apellido}",
                CalificacionProveedor = servicio.Usuario.Perfil?.CalificacionPromedio ?? 0,
                TotalResenasProveedor = servicio.Usuario.Perfil?.TotalResenas ?? 0,
                ProvinciaId = servicio.Usuario.Ubicacion?.ProvinciaId,
                Provincia = servicio.Usuario.Ubicacion?.Provincia?.Nombre,
                UbicacionProveedor = ConstruirUbicacionProveedor(servicio.Usuario.Ubicacion),
                Precio = servicio.PrecioBase,
                PrecioNegociable = servicio.PrecioNegociable,
                FechaCreacion = servicio.FechaCreacion
            };

            return Result<ServicioDetalleDto>.Ok(dto);
        }

        public async Task<Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>> ObtenerMisServiciosAsync(Guid usuarioId,
            ServicioFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            if (filtro.CalificacionMinima is < 0 or > 5)
                return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Fail(
                    "La calificación mínima debe estar entre 0 y 5.");

            if (filtro.ProvinciaId == Guid.Empty)
                return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Fail(
                    "La provincia no es válida.");

            if (filtro.CategoriaServicioIds?.Any(x => x == Guid.Empty) == true)
                return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Fail(
                    "La lista de categorías contiene un identificador no válido.");

            var texto = filtro.Busqueda?.Trim().ToLower();
            var categoriaServicioIds = filtro.CategoriaServicioIds?
                .Distinct()
                .ToArray();
            var ordenarPor = filtro.OrdenarPor?.Trim().ToLower();

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.UsuarioId == usuarioId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .WithCondition(
                            x => x.Titulo.ToLower().Contains(texto!) ||
                                 x.Descripcion.ToLower().Contains(texto!) ||
                                 x.CategoriaServicio.Nombre.ToLower().Contains(texto!),
                            !string.IsNullOrWhiteSpace(texto))

                        .WithCondition(
                            x => categoriaServicioIds!.Contains(x.CategoriaServicioId),
                            categoriaServicioIds?.Length > 0)

                        .WithCondition(
                            x => x.PrecioBase != null && x.PrecioBase <= filtro.PrecioMaximo,
                            filtro.PrecioMaximo is not null)

                        .WithCondition(
                            x => x.Usuario.Perfil != null &&
                                 x.Usuario.Perfil.CalificacionPromedio >= filtro.CalificacionMinima,
                            filtro.CalificacionMinima is not null)

                        .WithCondition(
                            x => x.Usuario.Ubicacion != null &&
                                 x.Usuario.Ubicacion.ProvinciaId == filtro.ProvinciaId,
                            filtro.ProvinciaId is not null)

                        .WithOrderBy(
                            x => x.PrecioBase,
                            ordenarPor == "precio_menor",
                            "asc")

                        .WithOrderBy(
                            x => x.PrecioBase,
                            ordenarPor == "precio_mayor",
                            "desc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "antiguos",
                            "asc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "recientes" || string.IsNullOrWhiteSpace(ordenarPor),
                            "desc"),
                    includes:
                    [
                        x => x.Usuario,
                        x => x.Usuario.Perfil!,
                        x => x.Usuario.Ubicacion!,
                        x => x.Usuario.Ubicacion!.Provincia,
                        x => x.Usuario.Ubicacion!.Municipio,
                        x => x.Usuario.Ubicacion!.Sector!,
                        x => x.CategoriaServicio
                    ]);

            var totalRecords = servicios.Count;

            var serviciosPaginados = servicios
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(x => new ServicioListadoDto
                {
                    Id = x.Id,
                    TituloServicio = x.Titulo,
                    Descripcion = x.Descripcion,
                    Categoria = x.CategoriaServicio.Nombre,
                    Proveedor = $"{x.Usuario.Nombre} {x.Usuario.Apellido}",
                    CalificacionProveedor = x.Usuario.Perfil?.CalificacionPromedio ?? 0,
                    TotalResenasProveedor = x.Usuario.Perfil?.TotalResenas ?? 0,
                    ProvinciaId = x.Usuario.Ubicacion?.ProvinciaId,
                    Provincia = x.Usuario.Ubicacion?.Provincia?.Nombre,
                    UbicacionProveedor = ConstruirUbicacionProveedor(x.Usuario.Ubicacion),
                    Precio = x.PrecioBase,
                    PrecioNegociable = x.PrecioNegociable,
                    FechaCreacion = x.FechaCreacion
                })
                .ToList();

            var pagedResponse = PagedResponse<IReadOnlyList<ServicioListadoDto>>.Create(
                serviciosPaginados,
                filtro.Page,
                filtro.PageSize,
                totalRecords);

            return Result<PagedResponse<IReadOnlyList<ServicioListadoDto>>>.Ok(pagedResponse);
        }

        private static string ConstruirUbicacionProveedor(UbicacionUsuario? ubicacion)
        {
            var partes = new[]
                {
                    ubicacion?.Sector?.Nombre,
                    ubicacion?.Municipio?.Nombre,
                    ubicacion?.Provincia?.Nombre
                }
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var ubicacionCompleta = string.Join(", ", partes);

            return string.IsNullOrWhiteSpace(ubicacionCompleta)
                ? "Ubicación no especificada"
                : ubicacionCompleta;
        }
    }
}
