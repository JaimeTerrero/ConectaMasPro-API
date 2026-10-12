using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Catalogos;
using ConectaMasPro.Application.DTOs.Ubicaciones;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Application.Interfaces.Services;
using System.Globalization;
using System.Text;

namespace ConectaMasPro.Application.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UbicacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerProvinciasAsync(
            CancellationToken cancellationToken = default)
        {
            var provincias = await _unitOfWork
                .Repository<Provincia>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .OrderBy(x => x.Nombre),
                    includes: []);

            var response = provincias
                .Select(x => new CatalogoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToList();

            return Result<IReadOnlyList<CatalogoDto>>.Ok(response);
        }

        public async Task<Result<IReadOnlyList<CatalogoDto>>> ObtenerMunicipiosPorProvinciaAsync(
            Guid provinciaId,
            CancellationToken cancellationToken = default)
        {
            if (provinciaId == Guid.Empty)
                return Result<IReadOnlyList<CatalogoDto>>
                    .Fail("La provincia es obligatoria.");

            var municipios = await _unitOfWork
                .Repository<Municipio>()
                .GetWithQueryAsync(
                    predicate: x => x.ProvinciaId == provinciaId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .OrderBy(x => x.Nombre),
                    includes: []);

            var response = municipios
                .Select(x => new CatalogoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToList();

            return Result<IReadOnlyList<CatalogoDto>>.Ok(response);
        }

        public async Task<Result<IReadOnlyList<SectorCatalogoDto>>> ObtenerSectoresPorMunicipioAsync(
            Guid municipioId,
            CancellationToken cancellationToken = default)
        {
            if (municipioId == Guid.Empty)
                return Result<IReadOnlyList<SectorCatalogoDto>>
                    .Fail("El municipio es obligatorio.");

            var sectores = await _unitOfWork
                .Repository<Sector>()
                .GetWithQueryAsync(
                    predicate: x => x.MunicipioId == municipioId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .OrderBy(x => x.Nombre),
                    includes: []);

            var response = sectores
                .Select(x => new SectorCatalogoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    LatitudCentro = x.LatitudCentro,
                    LongitudCentro = x.LongitudCentro
                })
                .ToList();

            return Result<IReadOnlyList<SectorCatalogoDto>>.Ok(response);
        }

        public async Task<Result<UbicacionCatalogoResueltaDto>> ResolverUbicacionGoogleAsync(
            ResolverUbicacionGoogleRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.Latitud is < 18.30m or > 18.75m ||
                request.Longitud is < -70.20m or > -69.45m)
            {
                return Result<UbicacionCatalogoResueltaDto>.Fail(
                    "Las coordenadas deben encontrarse dentro del Gran Santo Domingo.");
            }

            var provincias = await _unitOfWork
                .Repository<Provincia>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    queryModifier: query => query.OrderBy(x => x.Nombre),
                    cancellationToken: cancellationToken,
                    includes: []);

            var municipios = await _unitOfWork
                .Repository<Municipio>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    queryModifier: query => query.OrderBy(x => x.Nombre),
                    cancellationToken: cancellationToken,
                    includes: []);

            var provinciaNormalizada = NormalizarNombreGeografico(request.Provincia);
            var municipioNormalizado = ResolverAliasMunicipio(
                provinciaNormalizada,
                NormalizarNombreGeografico(request.Municipio));

            var provincia = CoincidenciaUnica(
                provincias,
                provinciaNormalizada,
                x => x.Nombre);

            Municipio? municipio = null;

            if (provincia is not null)
            {
                municipio = CoincidenciaUnica(
                    municipios.Where(x => x.ProvinciaId == provincia.Id),
                    municipioNormalizado,
                    x => x.Nombre);
            }
            else
            {
                // Si Google no entrega una provincia utilizable, un municipio
                // inequívoco todavía permite recuperar la provincia del catálogo.
                municipio = CoincidenciaUnica(
                    municipios,
                    municipioNormalizado,
                    x => x.Nombre);

                if (municipio is not null)
                {
                    provincia = provincias.FirstOrDefault(
                        x => x.Id == municipio.ProvinciaId);
                }
            }

            var sectores = Array.Empty<Sector>();
            Sector? sector = null;

            if (municipio is not null)
            {
                sectores = (await _unitOfWork
                    .Repository<Sector>()
                    .GetWithQueryAsync(
                        predicate: x => x.MunicipioId == municipio.Id && x.Activo,
                        orderBy: null,
                        queryModifier: query => query.OrderBy(x => x.Nombre),
                        cancellationToken: cancellationToken,
                        includes: []))
                    .ToArray();

                sector = CoincidenciaUnica(
                    sectores,
                    NormalizarNombreGeografico(request.Sector),
                    x => x.Nombre);
            }

            var motivos = new List<string>();

            if (provincia is null)
                motivos.Add("No se pudo relacionar la provincia de Google con el catálogo interno.");

            if (municipio is null)
                motivos.Add("No se pudo relacionar el municipio de Google con el catálogo interno.");

            if (!string.IsNullOrWhiteSpace(request.Sector) && sector is null)
                motivos.Add("No se pudo relacionar el sector de Google con el catálogo interno.");

            var response = new UbicacionCatalogoResueltaDto
            {
                Latitud = request.Latitud,
                Longitud = request.Longitud,
                ProvinciaId = provincia?.Id,
                Provincia = provincia?.Nombre,
                MunicipioId = municipio?.Id,
                Municipio = municipio?.Nombre,
                SectorId = sector?.Id,
                Sector = sector?.Nombre,
                RequiereConfirmacion = motivos.Count > 0,
                MotivoConfirmacion = motivos.Count > 0
                    ? string.Join(" ", motivos)
                    : null,
                ProvinciasSugeridas = provincia is null
                    ? provincias.Select(MapearCatalogo).ToList()
                    : [],
                MunicipiosSugeridos = municipio is null && provincia is not null
                    ? municipios
                        .Where(x => x.ProvinciaId == provincia.Id)
                        .Select(MapearCatalogo)
                        .ToList()
                    : [],
                SectoresSugeridos = municipio is not null &&
                                     !string.IsNullOrWhiteSpace(request.Sector) &&
                                     sector is null
                    ? sectores.Select(MapearSector).ToList()
                    : []
            };

            return Result<UbicacionCatalogoResueltaDto>.Ok(response);
        }

        private static T? CoincidenciaUnica<T>(
            IEnumerable<T> elementos,
            string nombreNormalizado,
            Func<T, string> obtenerNombre)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
                return null;

            var coincidencias = elementos
                .Where(x => NormalizarNombreGeografico(obtenerNombre(x)) == nombreNormalizado)
                .Take(2)
                .ToList();

            return coincidencias.Count == 1
                ? coincidencias[0]
                : null;
        }

        private static string NormalizarNombreGeografico(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            var normalizado = valor
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder(normalizado.Length);

            foreach (var caracter in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) ==
                    UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                builder.Append(char.IsLetterOrDigit(caracter) ? caracter : ' ');
            }

            var palabrasIgnoradas = new HashSet<string>
            {
                "provincia",
                "municipio"
            };

            return string.Join(
                " ",
                builder
                    .ToString()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !palabrasIgnoradas.Contains(x)));
        }

        private static string ResolverAliasMunicipio(
            string provinciaNormalizada,
            string municipioNormalizado)
        {
            if (provinciaNormalizada == "distrito nacional" &&
                municipioNormalizado == "santo domingo de guzman")
            {
                return "distrito nacional";
            }

            return municipioNormalizado;
        }

        private static CatalogoDto MapearCatalogo(Provincia provincia) => new()
        {
            Id = provincia.Id,
            Nombre = provincia.Nombre
        };

        private static CatalogoDto MapearCatalogo(Municipio municipio) => new()
        {
            Id = municipio.Id,
            Nombre = municipio.Nombre
        };

        private static SectorCatalogoDto MapearSector(Sector sector) => new()
        {
            Id = sector.Id,
            Nombre = sector.Nombre,
            LatitudCentro = sector.LatitudCentro,
            LongitudCentro = sector.LongitudCentro
        };
    }
}
