using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Perfiles;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IArchivoStorage _archivoStorage;

        public PerfilService(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IArchivoStorage archivoStorage)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _archivoStorage = archivoStorage;
        }

        public async Task<Result<PerfilClienteDto>> ObtenerPerfilClienteAsync(
    Guid usuarioId,
    CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
            {
                return Result<PerfilClienteDto>.Fail(
                    "El usuario autenticado no es válido.");
            }

            var usuarios = await _unitOfWork
                .Repository<Usuario>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == usuarioId && x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Perfil!,
                x => x.Ubicacion!,
                x => x.Ubicacion!.Provincia,
                x => x.Ubicacion!.Municipio,
                x => x.Ubicacion!.Sector!
                    ]);

            var usuario = usuarios.FirstOrDefault();

            if (usuario is null)
            {
                return Result<PerfilClienteDto>.Fail(
                    "No se encontró el perfil del usuario.");
            }

            var twoFactorEnabled = await _identityService.TieneDosFactoresHabilitadoAsync(usuario.IdentityUserId);

            var usuarioIntereses = await _unitOfWork
                .Repository<UsuarioInteres>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.UsuarioId == usuarioId &&
                        x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Interes
                    ]);

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.ClienteId == usuarioId &&
                        x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Pago!
                    ]);

            var solicitudesActivas = solicitudes.Count(x =>
                x.Estado == EstadoSolicitudServicio.Pendiente ||
                x.Estado == EstadoSolicitudServicio.Aceptada);

            var serviciosCompletados = solicitudes.Count(x =>
                x.Estado == EstadoSolicitudServicio.Completada);

            var pagosRealizados = solicitudes.Count(x =>
                x.Pago is not null &&
                x.Pago.Estado == EstadoPago.Pagado);

            var montoTotalPagado = solicitudes
                .Where(x =>
                    x.Pago is not null &&
                    x.Pago.Estado == EstadoPago.Pagado)
                .Sum(x => x.Pago!.Monto);

            var perfil = usuario.Perfil;
            var ubicacion = usuario.Ubicacion;

            var ubicacionCompleta = ConstruirUbicacion(
                ubicacion?.Sector?.Nombre,
                ubicacion?.Municipio?.Nombre,
                ubicacion?.Provincia?.Nombre);

            var ubicacionMapa = ConstruirUbicacionMapa(ubicacion);
            var resenasRecientes = await ObtenerResenasRecientesAsync(usuarioId, cancellationToken);

            var response = new PerfilClienteDto
            {
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Telefono = usuario.Telefono,

                TipoUsuarioId = (int)usuario.TipoUsuario,
                TipoUsuario = usuario.TipoUsuario.ToString(),

                FotoUrl = perfil?.FotoUrl,
                IdentidadVerificada = perfil?.IdentidadVerificada ?? false,

                Provincia = ubicacion?.Provincia?.Nombre,
                ProvinciaId = ubicacion?.ProvinciaId,
                Municipio = ubicacion?.Municipio?.Nombre,
                MunicipioId = ubicacion?.MunicipioId,
                Sector = ubicacion?.Sector?.Nombre,
                SectorId = ubicacion?.SectorId,
                UbicacionCompleta = ubicacionCompleta,
                UbicacionMapa = ubicacionMapa,

                Biografia = perfil?.Biografia,

                CalificacionPromedio = perfil?.CalificacionPromedio ?? 0,
                TotalResenas = perfil?.TotalResenas ?? 0,
                ResenasRecientes = resenasRecientes,

                SolicitudesActivas = solicitudesActivas,
                ServiciosCompletados = serviciosCompletados,
                PagosRealizados = pagosRealizados,
                MontoTotalPagado = montoTotalPagado,

                MiembroDesde = usuario.FechaCreacion,
                AutenticacionDosFactoresHabilitada = twoFactorEnabled,

                Intereses = usuarioIntereses
                    .Where(x =>
                        x.Interes is not null &&
                        x.Interes.Activo)
                    .OrderBy(x => x.Interes.Nombre)
                    .Select(x => new InteresPerfilDto
                    {
                        Id = x.InteresId,
                        Nombre = x.Interes.Nombre
                    })
                    .ToList()
            };

            return Result<PerfilClienteDto>.Ok(response);
        }

        private static string? ConstruirUbicacion(
            string? sector,
            string? municipio,
            string? provincia)
        {
            var partes = new[] { sector, municipio, provincia }
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var ubicacion = string.Join(", ", partes);

            return string.IsNullOrWhiteSpace(ubicacion)
                ? null
                : ubicacion;
        }

        private static UbicacionMapaDto? ConstruirUbicacionMapa(
            UbicacionUsuario? ubicacion)
        {
            if (ubicacion?.Latitud is not null && ubicacion.Longitud is not null)
            {
                return new UbicacionMapaDto
                {
                    Latitud = ubicacion.Latitud.Value,
                    Longitud = ubicacion.Longitud.Value,
                    Precision = ubicacion.UbicacionExacta ? "Exacta" : "Aproximada",
                    EsExacta = ubicacion.UbicacionExacta
                };
            }

            if (ubicacion?.Sector?.LatitudCentro is not null &&
                ubicacion.Sector.LongitudCentro is not null)
            {
                return new UbicacionMapaDto
                {
                    Latitud = ubicacion.Sector.LatitudCentro.Value,
                    Longitud = ubicacion.Sector.LongitudCentro.Value,
                    Precision = "Sector",
                    EsExacta = false
                };
            }

            return null;
        }

        public async Task<Result<PerfilProveedorDto>> ObtenerPerfilProveedorAsync(
    Guid usuarioId,
    CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
            {
                return Result<PerfilProveedorDto>.Fail(
                    "El usuario autenticado no es válido.");
            }

            var usuarios = await _unitOfWork
                .Repository<Usuario>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == usuarioId && x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Perfil!,
                x => x.Ubicacion!,
                x => x.Ubicacion!.Provincia,
                x => x.Ubicacion!.Municipio,
                x => x.Ubicacion!.Sector!
                    ]);

            var usuario = usuarios.FirstOrDefault();

            if (usuario is null)
            {
                return Result<PerfilProveedorDto>.Fail(
                    "No se encontró el perfil del usuario.");
            }

            var twoFactorEnabled = await _identityService.TieneDosFactoresHabilitadoAsync(usuario.IdentityUserId);

            if (usuario.TipoUsuario != TipoUsuario.Proveedor &&
                usuario.TipoUsuario != TipoUsuario.Ambos &&
                usuario.TipoUsuario != TipoUsuario.Administrador)
            {
                return Result<PerfilProveedorDto>.Fail(
                    "El usuario no posee un perfil de proveedor.");
            }

            var usuarioIntereses = await _unitOfWork
                .Repository<UsuarioInteres>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.UsuarioId == usuarioId &&
                        x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Interes
                    ]);

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.UsuarioId == usuarioId &&
                        x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes: []);

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.Servicio.UsuarioId == usuarioId &&
                        x.Activo,
                    orderBy: null,
                    queryModifier: null,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Servicio
                    ]);

            var serviciosActivos = servicios.Count;
            var solicitudesRecibidas = solicitudes.Count;

            var serviciosCompletados = solicitudes.Count(x =>
                x.Estado == EstadoSolicitudServicio.Completada);

            var perfil = usuario.Perfil;
            var ubicacion = usuario.Ubicacion;

            var ubicacionCompleta = ConstruirUbicacion(
                ubicacion?.Sector?.Nombre,
                ubicacion?.Municipio?.Nombre,
                ubicacion?.Provincia?.Nombre);

            var ubicacionMapa = ConstruirUbicacionMapa(ubicacion);
            var resenasRecientes = await ObtenerResenasRecientesAsync(usuarioId, cancellationToken);

            var response = new PerfilProveedorDto
            {
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                TipoUsuarioId = (int)usuario.TipoUsuario,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                FotoUrl = perfil?.FotoUrl,
                IdentidadVerificada = perfil?.IdentidadVerificada ?? false,
                Provincia = ubicacion?.Provincia?.Nombre,
                ProvinciaId = ubicacion?.ProvinciaId,
                Municipio = ubicacion?.Municipio?.Nombre,
                MunicipioId = ubicacion?.MunicipioId,
                Sector = ubicacion?.Sector?.Nombre,
                SectorId = ubicacion?.SectorId,
                UbicacionCompleta = ubicacionCompleta,
                UbicacionMapa = ubicacionMapa,
                Biografia = perfil?.Biografia,
                ServiciosActivos = serviciosActivos,
                SolicitudesRecibidas = solicitudesRecibidas,
                ServiciosCompletados = serviciosCompletados,
                CalificacionPromedio = perfil?.CalificacionPromedio ?? 0,
                TotalResenas = perfil?.TotalResenas ?? 0,
                ResenasRecientes = resenasRecientes,
                MiembroDesde = usuario.FechaCreacion,
                AutenticacionDosFactoresHabilitada = twoFactorEnabled,
                Intereses = usuarioIntereses
                    .Where(x =>
                        x.Interes is not null &&
                        x.Interes.Activo)
                    .OrderBy(x => x.Interes.Nombre)
                    .Select(x => new InteresPerfilDto
                    {
                        Id = x.InteresId,
                        Nombre = x.Interes.Nombre
                    })
                    .ToList()
            };

            return Result<PerfilProveedorDto>.Ok(response);
        }

        public Task<Result<PerfilPublicoDto>> ObtenerPerfilPublicoClienteAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default) =>
            ObtenerPerfilPublicoAsync(usuarioId, esProveedor: false, cancellationToken);

        public Task<Result<PerfilPublicoDto>> ObtenerPerfilPublicoProveedorAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default) =>
            ObtenerPerfilPublicoAsync(usuarioId, esProveedor: true, cancellationToken);

        private async Task<Result<PerfilPublicoDto>> ObtenerPerfilPublicoAsync(
            Guid usuarioId,
            bool esProveedor,
            CancellationToken cancellationToken)
        {
            if (usuarioId == Guid.Empty)
                return Result<PerfilPublicoDto>.Fail("El usuario no es válido.");

            var usuarios = await _unitOfWork.Repository<Usuario>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == usuarioId && x.Activo,
                    cancellationToken: cancellationToken,
                    includes:
                    [
                        x => x.Perfil!,
                        x => x.Ubicacion!,
                        x => x.Ubicacion!.Provincia,
                        x => x.Ubicacion!.Municipio,
                        x => x.Ubicacion!.Sector!
                    ]);

            var usuario = usuarios.FirstOrDefault();
            if (usuario is null)
                return Result<PerfilPublicoDto>.Fail("No se encontró el perfil del usuario.");

            var tipoValido = esProveedor
                ? usuario.TipoUsuario is TipoUsuario.Proveedor or TipoUsuario.Ambos
                : usuario.TipoUsuario is TipoUsuario.Cliente or TipoUsuario.Ambos;

            if (!tipoValido)
                return Result<PerfilPublicoDto>.Fail(
                    esProveedor
                        ? "El usuario no posee un perfil de proveedor."
                        : "El usuario no posee un perfil de cliente.");

            var solicitudes = await _unitOfWork.Repository<SolicitudServicio>()
                .FindAsync(
                    esProveedor
                        ? x => x.Servicio.UsuarioId == usuarioId &&
                               x.Estado == EstadoSolicitudServicio.Completada && x.Activo
                        : x => x.ClienteId == usuarioId &&
                               x.Estado == EstadoSolicitudServicio.Completada && x.Activo,
                    cancellationToken);

            var perfil = usuario.Perfil;
            var ubicacion = usuario.Ubicacion;

            return Result<PerfilPublicoDto>.Ok(new PerfilPublicoDto
            {
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                TipoUsuarioId = (int)usuario.TipoUsuario,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                FotoUrl = perfil?.FotoUrl,
                IdentidadVerificada = perfil?.IdentidadVerificada ?? false,
                UbicacionCompleta = ConstruirUbicacion(
                    ubicacion?.Sector?.Nombre,
                    ubicacion?.Municipio?.Nombre,
                    ubicacion?.Provincia?.Nombre),
                Biografia = perfil?.Biografia,
                CalificacionPromedio = perfil?.CalificacionPromedio ?? 0,
                TotalResenas = perfil?.TotalResenas ?? 0,
                ServiciosCompletados = solicitudes.Count,
                MiembroDesde = usuario.FechaCreacion,
                ResenasRecientes = await ObtenerResenasRecientesAsync(usuarioId, cancellationToken)
            });
        }

        private async Task<IReadOnlyList<ResenaRecientePerfilDto>> ObtenerResenasRecientesAsync(
            Guid usuarioId,
            CancellationToken cancellationToken)
        {
            var resenas = await _unitOfWork.Repository<Resena>()
                .GetWithQueryAsync(
                    predicate: x => x.EvaluadoId == usuarioId && x.Activo,
                    queryModifier: query => query
                        .OrderByDescending(x => x.FechaCreacion)
                        .Take(5),
                    cancellationToken: cancellationToken,
                    includes: [x => x.Evaluador]);

            return resenas.Select(x => new ResenaRecientePerfilDto
            {
                Id = x.Id,
                EvaluadorId = x.EvaluadorId,
                Evaluador = $"{x.Evaluador.Nombre} {x.Evaluador.Apellido}",
                CalificacionGeneral = x.CalificacionGeneral,
                Comentario = x.Comentario,
                FechaCreacion = x.FechaCreacion
            }).ToList();
        }

        public async Task<Result<PerfilActualizadoDto>> ActualizarMiPerfilAsync(
        Guid usuarioId,
        ActualizarPerfilRequest request,
        CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "El usuario autenticado no es válido.");
            }

            if (request.ProvinciaId == Guid.Empty)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "La provincia es obligatoria.");
            }

            if (request.MunicipioId == Guid.Empty)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "El municipio es obligatorio.");
            }

            if (request.Latitud.HasValue != request.Longitud.HasValue)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "La latitud y la longitud deben proporcionarse juntas.");
            }

            if (request.UbicacionExacta && !request.Latitud.HasValue)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "Debe proporcionar coordenadas para marcar la ubicación como exacta.");
            }

            if (request.Latitud is < 18.30m or > 18.75m ||
                request.Longitud is < -70.20m or > -69.45m)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "Las coordenadas deben encontrarse dentro del Gran Santo Domingo.");
            }

            if (!string.IsNullOrWhiteSpace(request.Biografia) &&
                request.Biografia.Trim().Length > 500)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "La biografía no puede superar los 500 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(request.Telefono) &&
                request.Telefono.Trim().Length > 20)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "El teléfono no puede superar los 20 caracteres.");
            }

            var usuario = await _unitOfWork
                .Repository<Usuario>()
                .GetByIdAsync(usuarioId, cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "El usuario no existe o está inactivo.");
            }

            var provincia = await _unitOfWork
                .Repository<Provincia>()
                .GetByIdAsync(request.ProvinciaId, cancellationToken);

            if (provincia is null || !provincia.Activo)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "La provincia seleccionada no existe o está inactiva.");
            }

            var municipios = await _unitOfWork
                .Repository<Municipio>()
                .FindAsync(
                    x => x.Id == request.MunicipioId &&
                         x.ProvinciaId == request.ProvinciaId &&
                         x.Activo,
                    cancellationToken);

            var municipio = municipios.FirstOrDefault();

            if (municipio is null)
            {
                return Result<PerfilActualizadoDto>.Fail(
                    "El municipio no pertenece a la provincia seleccionada.");
            }

            Sector? sector = null;

            if (request.SectorId.HasValue)
            {
                var sectores = await _unitOfWork
                    .Repository<Sector>()
                    .FindAsync(
                        x => x.Id == request.SectorId.Value &&
                             x.MunicipioId == request.MunicipioId &&
                             x.Activo,
                        cancellationToken);

                sector = sectores.FirstOrDefault();

                if (sector is null)
                {
                    return Result<PerfilActualizadoDto>.Fail(
                        "El sector no pertenece al municipio seleccionado.");
                }
            }

            var interesesIds = request.InteresesIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            var interesesSeleccionados = new List<Interes>();

            if (interesesIds.Count > 0)
            {
                var interesesEncontrados = await _unitOfWork
                    .Repository<Interes>()
                    .FindAsync(
                        x => interesesIds.Contains(x.Id) && x.Activo,
                        cancellationToken);

                interesesSeleccionados = interesesEncontrados.ToList();

                if (interesesSeleccionados.Count != interesesIds.Count)
                {
                    return Result<PerfilActualizadoDto>.Fail(
                        "Uno o más intereses seleccionados no existen o están inactivos.");
                }
            }

            usuario.Telefono = string.IsNullOrWhiteSpace(request.Telefono)
                ? null
                : request.Telefono.Trim();

            var perfiles = await _unitOfWork
                .Repository<PerfilUsuario>()
                .FindAsync(
                    x => x.UsuarioId == usuarioId,
                    cancellationToken);

            var perfil = perfiles.FirstOrDefault();

            if (perfil is null)
            {
                perfil = new PerfilUsuario
                {
                    UsuarioId = usuarioId,
                    Biografia = string.IsNullOrWhiteSpace(request.Biografia)
                        ? null
                        : request.Biografia.Trim()
                };

                await _unitOfWork
                    .Repository<PerfilUsuario>()
                    .AddAsync(perfil, cancellationToken);
            }
            else
            {
                perfil.Biografia = string.IsNullOrWhiteSpace(request.Biografia)
                    ? null
                    : request.Biografia.Trim();

                perfil.FechaModificacion = DateTime.UtcNow;

                _unitOfWork
                  .Repository<PerfilUsuario>()
                  .Update(perfil);
            }

            var ubicaciones = await _unitOfWork
                .Repository<UbicacionUsuario>()
                .FindAsync(
                    x => x.UsuarioId == usuarioId,
                    cancellationToken);

            var ubicacion = ubicaciones.FirstOrDefault();

            if (ubicacion is null)
            {
                ubicacion = new UbicacionUsuario
                {
                    UsuarioId = usuarioId,
                    ProvinciaId = request.ProvinciaId,
                    MunicipioId = request.MunicipioId,
                    SectorId = request.SectorId,
                    Latitud = request.Latitud,
                    Longitud = request.Longitud,
                    UbicacionExacta = request.UbicacionExacta && request.Latitud.HasValue
                };

                await _unitOfWork
                    .Repository<UbicacionUsuario>()
                    .AddAsync(ubicacion, cancellationToken);
            }
            else
            {
                ubicacion.ProvinciaId = request.ProvinciaId;
                ubicacion.MunicipioId = request.MunicipioId;
                ubicacion.SectorId = request.SectorId;
                ubicacion.Latitud = request.Latitud;
                ubicacion.Longitud = request.Longitud;
                ubicacion.UbicacionExacta = request.UbicacionExacta && request.Latitud.HasValue;
                ubicacion.FechaModificacion = DateTime.UtcNow;

                _unitOfWork
                   .Repository<UbicacionUsuario>()
                   .Update(ubicacion);
            }

            var interesesActuales = await _unitOfWork
                .Repository<UsuarioInteres>()
                .FindAsync(
                    x => x.UsuarioId == usuarioId,
                    cancellationToken);

            foreach (var usuarioInteres in interesesActuales)
            {
                usuarioInteres.Activo = interesesIds.Contains(
                    usuarioInteres.InteresId);

                usuarioInteres.FechaModificacion = DateTime.UtcNow;
            }

            var interesesActualesIds = interesesActuales
                .Select(x => x.InteresId)
                .ToHashSet();

            var nuevosInteresesIds = interesesIds
                .Where(x => !interesesActualesIds.Contains(x))
                .ToList();

            foreach (var interesId in nuevosInteresesIds)
            {
                var usuarioInteres = new UsuarioInteres
                {
                    UsuarioId = usuarioId,
                    InteresId = interesId
                };

                await _unitOfWork
                    .Repository<UsuarioInteres>()
                    .AddAsync(usuarioInteres, cancellationToken);
            }

            usuario.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var ubicacionCompleta = string.Join(
                ", ",
                new[]
                {
            sector?.Nombre,
            municipio.Nombre,
            provincia.Nombre
                }.Where(x => !string.IsNullOrWhiteSpace(x)));

            var response = new PerfilActualizadoDto
            {
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Telefono = usuario.Telefono,

                ProvinciaId = provincia.Id,
                Provincia = provincia.Nombre,

                MunicipioId = municipio.Id,
                Municipio = municipio.Nombre,

                SectorId = sector?.Id,
                Sector = sector?.Nombre,

                UbicacionCompleta = ubicacionCompleta,
                UbicacionMapa = ConstruirUbicacionMapa(ubicacion),
                Biografia = perfil.Biografia,
                FotoUrl = perfil.FotoUrl,

                Intereses = interesesSeleccionados
                    .OrderBy(x => x.Nombre)
                    .Select(x => new InteresPerfilDto
                    {
                        Id = x.Id,
                        Nombre = x.Nombre
                    })
                    .ToList()
            };

            return Result<PerfilActualizadoDto>.Ok(response, "Los datos fueron actualizados correctamente.");
        }

        public async Task<Result<FotoPerfilDto>> ActualizarFotoPerfilAsync(
            Guid usuarioId,
            ActualizarFotoPerfilRequest request,
            CancellationToken cancellationToken = default)
        {
            if (usuarioId == Guid.Empty)
                return Result<FotoPerfilDto>.Fail("El usuario autenticado no es válido.");

            if (request.Contenido.Length == 0)
                return Result<FotoPerfilDto>.Fail("La foto de perfil es obligatoria.");

            if (request.Contenido.Length > 5 * 1024 * 1024)
                return Result<FotoPerfilDto>.Fail("La foto de perfil no puede superar 5 MB.");

            string[] tiposPermitidos = ["image/jpeg", "image/png", "image/webp"];
            if (!tiposPermitidos.Contains(request.TipoContenido, StringComparer.OrdinalIgnoreCase))
                return Result<FotoPerfilDto>.Fail("El tipo de imagen no está permitido.");

            var usuario = await _unitOfWork.Repository<Usuario>()
                .GetByIdAsync(usuarioId, cancellationToken);

            if (usuario is null || !usuario.Activo)
                return Result<FotoPerfilDto>.Fail("El usuario no existe o está inactivo.");

            var perfiles = await _unitOfWork.Repository<PerfilUsuario>()
                .FindAsync(x => x.UsuarioId == usuarioId, cancellationToken);
            var perfil = perfiles.FirstOrDefault();

            var fotoAnterior = perfil?.FotoUrl;
            var nuevaFotoUrl = await _archivoStorage.GuardarImagenAsync(
                request.Contenido,
                request.TipoContenido,
                "perfiles",
                cancellationToken);

            if (perfil is null)
            {
                perfil = new PerfilUsuario { UsuarioId = usuarioId };
                await _unitOfWork.Repository<PerfilUsuario>().AddAsync(perfil, cancellationToken);
            }
            else
            {
                perfil.FechaModificacion = DateTime.UtcNow;
                _unitOfWork.Repository<PerfilUsuario>().Update(perfil);
            }

            perfil.FotoUrl = nuevaFotoUrl;

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _archivoStorage.EliminarAsync(nuevaFotoUrl, CancellationToken.None);
                throw;
            }

            await _archivoStorage.EliminarAsync(fotoAnterior, cancellationToken);

            return Result<FotoPerfilDto>.Ok(
                new FotoPerfilDto
                {
                    FotoUrl = perfil.FotoUrl
                },
                "La foto de perfil fue actualizada correctamente.");
        }
    }
}
