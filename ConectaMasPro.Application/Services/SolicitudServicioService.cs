using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Extensions;
using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Application.DTOs.Solicitudes;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class SolicitudServicioService : ISolicitudServicioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecompensaService _recompensaService;
        private readonly IProgresoProveedorService _progresoProveedorService;

        public SolicitudServicioService(IUnitOfWork unitOfWork, IRecompensaService recompensaService, IProgresoProveedorService progresoProveedorService)
        {
            _unitOfWork = unitOfWork;
            _recompensaService = recompensaService;
            _progresoProveedorService = progresoProveedorService;
        }

        public async Task<Result<SolicitudServicioListadoDto>> CrearAsync(Guid clienteId, CrearSolicitudServicioRequest request,
        CancellationToken cancellationToken = default)
        {
            if (clienteId == Guid.Empty)
                return Result<SolicitudServicioListadoDto>.Fail("El cliente autenticado no es válido.");

            if (request.ServicioId == Guid.Empty)
                return Result<SolicitudServicioListadoDto>.Fail("El servicio es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.DescripcionSolicitud))
                return Result<SolicitudServicioListadoDto>.Fail("La descripción de la solicitud es obligatoria.");

            var cliente = await _unitOfWork
                .Repository<Usuario>()
                .GetByIdAsync(clienteId, cancellationToken);

            if (cliente is null || !cliente.Activo)
                return Result<SolicitudServicioListadoDto>.Fail("El cliente no existe.");

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.Id == request.ServicioId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: null,
                    includes:
                    [
                        x => x.Usuario,
                x => x.Usuario.Perfil!,
                x => x.CategoriaServicio
                    ]);

            var servicio = servicios.FirstOrDefault();

            if (servicio is null)
                return Result<SolicitudServicioListadoDto>.Fail("El servicio no existe.");

            var solicitud = new SolicitudServicio
            {
                ClienteId = clienteId,
                ServicioId = request.ServicioId,
                DescripcionSolicitud = request.DescripcionSolicitud.Trim(),
                FechaDeseada = request.FechaDeseada,
                PrecioAcordado = ObtenerPrecioVisible(null, servicio),
                Estado = EstadoSolicitudServicio.Pendiente
            };

            await _unitOfWork.Repository<SolicitudServicio>()
                .AddAsync(solicitud, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new SolicitudServicioListadoDto
            {
                Id = solicitud.Id,
                ServicioId = solicitud.ServicioId,
                TituloServicio = servicio.Titulo,
                Categoria = servicio.CategoriaServicio.Nombre,
                ProveedorId = servicio.UsuarioId,
                Proveedor = $"{servicio.Usuario.Nombre} {servicio.Usuario.Apellido}",
                CalificacionProveedor = servicio.Usuario.Perfil?.CalificacionPromedio ?? 0,
                TotalResenasProveedor = servicio.Usuario.Perfil?.TotalResenas ?? 0,
                DescripcionSolicitud = solicitud.DescripcionSolicitud,
                FechaDeseada = solicitud.FechaDeseada,
                PrecioAcordado = ObtenerPrecioVisible(solicitud.PrecioAcordado, servicio),
                EstadoId = (int)solicitud.Estado,
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                AvancePorcentaje = ObtenerAvancePorEstado(solicitud.Estado)
            };

            return Result<SolicitudServicioListadoDto>.Ok(dto);
        }

        public async Task<Result<CambiarEstadoSolicitudResponse>> CambiarEstadoAsync(Guid solicitudId, Guid proveedorId, EstadoSolicitudServicio estado, CancellationToken cancellationToken = default)
        {
            if (proveedorId == Guid.Empty)
            {
                return Result<CambiarEstadoSolicitudResponse>.Fail(
                    "El proveedor autenticado no es válido.");
            }

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetAsync(
                    x =>
                        x.Id == solicitudId &&
                        x.Activo,

                    null,

                    cancellationToken,

                    x => x.Servicio,
                    x => x.Cliente);

            var solicitud =
                solicitudes.FirstOrDefault();

            if (solicitud is null)
            {
                return Result<CambiarEstadoSolicitudResponse>.Fail(
                    "Solicitud no encontrada.");
            }

            /*
             * Seguridad:
             *
             * solamente el dueño del servicio
             * puede cambiar el estado.
             */
            if (solicitud.Servicio.UsuarioId != proveedorId)
            {
                return Result<CambiarEstadoSolicitudResponse>.Fail(
                    "No tienes permiso para modificar esta solicitud.");
            }

            var estadoAnterior =
                solicitud.Estado;

            if (estadoAnterior == estado)
            {
                Guid? conversacionExistenteId = null;
                if (estado == EstadoSolicitudServicio.Aceptada)
                {
                    var conversacionesExistentes = await _unitOfWork.Repository<Conversacion>()
                        .FindAsync(
                            x => x.SolicitudServicioId == solicitud.Id && x.Activo,
                            cancellationToken);
                    conversacionExistenteId = conversacionesExistentes.FirstOrDefault()?.Id;
                }

                return Result<CambiarEstadoSolicitudResponse>.Ok(
                    CrearRespuestaCambioEstado(solicitud, conversacionExistenteId),
                    "La solicitud ya se encuentra en ese estado.");
            }

            /*
             * Validamos transiciones.
             */
            if (!PuedeCambiarEstado(estadoAnterior, estado))
            {
                return Result<CambiarEstadoSolicitudResponse>.Fail(
                    $"No es posible cambiar la solicitud " +
                    $"de {estadoAnterior} a {estado}.");
            }

            if (estado == EstadoSolicitudServicio.Aceptada && !solicitud.Servicio.PrecioNegociable)
            {
                if (!solicitud.Servicio.PrecioBase.HasValue || solicitud.Servicio.PrecioBase <= 0)
                    return Result<CambiarEstadoSolicitudResponse>.Fail(
                        "El servicio no negociable debe tener un precio base válido antes de aceptar la solicitud.");

                solicitud.PrecioAcordado = decimal.Round(
                    solicitud.Servicio.PrecioBase.Value,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            if (estado == EstadoSolicitudServicio.PendienteConfirmacionCliente &&
                (!solicitud.PrecioAcordado.HasValue || solicitud.PrecioAcordado <= 0))
            {
                return Result<CambiarEstadoSolicitudResponse>.Fail(
                    solicitud.Servicio.PrecioNegociable
                        ? "Debes establecer el precio acordado antes de reportar la finalización."
                        : "El servicio no tiene un precio válido para continuar.");
            }

            solicitud.Estado = estado;

            solicitud.FechaModificacion =
                DateTime.UtcNow;

            _unitOfWork
                .Repository<SolicitudServicio>()
                .Update(solicitud);

            Guid? conversacionId = null;
            string? mensajeAutomatico = null;

            if (estado == EstadoSolicitudServicio.Aceptada)
            {
                var conversaciones = await _unitOfWork.Repository<Conversacion>().FindAsync(
                    x => x.SolicitudServicioId == solicitud.Id && x.Activo,
                    cancellationToken);
                var conversacion = conversaciones.FirstOrDefault();

                if (conversacion is null)
                {
                    conversacion = new Conversacion
                    {
                        SolicitudServicioId = solicitud.Id,
                        UsuarioUnoId = solicitud.ClienteId,
                        UsuarioDosId = proveedorId
                    };

                    await _unitOfWork.Repository<Conversacion>()
                        .AddAsync(conversacion, cancellationToken);
                }

                conversacionId = conversacion.Id;
                mensajeAutomatico = CrearMensajeInicial(solicitud);

                await _unitOfWork.Repository<Mensaje>().AddAsync(
                    new Mensaje
                    {
                        ConversacionId = conversacion.Id,
                        EmisorId = proveedorId,
                        Contenido = mensajeAutomatico,
                        Leido = false
                    },
                    cancellationToken);
            }

            if (estado == EstadoSolicitudServicio.PendienteConfirmacionCliente)
                solicitud.FechaReporteFinalizacionProveedor = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var mensaje = estado switch
            {
                EstadoSolicitudServicio.Aceptada =>
                    "La solicitud fue aceptada correctamente.",

                EstadoSolicitudServicio.PendienteConfirmacionCliente =>
                    "El servicio fue reportado como finalizado. Falta la confirmación del cliente.",

                EstadoSolicitudServicio.Cancelada =>
                    "La solicitud fue cancelada correctamente.",

                EstadoSolicitudServicio.Rechazada =>
                    "La solicitud fue rechazada correctamente.",

                _ =>
                    "El estado de la solicitud fue actualizado correctamente."
            };

            return Result<CambiarEstadoSolicitudResponse>.Ok(
                CrearRespuestaCambioEstado(solicitud, conversacionId, mensajeAutomatico),
                mensaje);
        }

        public async Task<Result> EstablecerPrecioAcordadoAsync(Guid solicitudId, Guid proveedorId,
            EstablecerPrecioAcordadoRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PrecioAcordado <= 0)
                return Result.Fail("El precio acordado debe ser mayor que cero.");
            var solicitud = (await _unitOfWork.Repository<SolicitudServicio>().GetAsync(
                x => x.Id == solicitudId && x.Activo, null, cancellationToken, x => x.Servicio)).FirstOrDefault();
            if (solicitud is null) return Result.Fail("Solicitud no encontrada.");
            if (solicitud.Servicio.UsuarioId != proveedorId)
                return Result.Fail("Solo el proveedor del servicio puede establecer el precio acordado.");
            if (!solicitud.Servicio.PrecioNegociable)
                return Result.Fail("Este servicio no es negociable; se utiliza automáticamente su precio base.");
            if (solicitud.Estado != EstadoSolicitudServicio.Aceptada)
                return Result.Fail("El precio solo puede establecerse mientras la solicitud está aceptada.");
            solicitud.PrecioAcordado = decimal.Round(request.PrecioAcordado, 2, MidpointRounding.AwayFromZero);
            solicitud.FechaModificacion = DateTime.UtcNow;
            _unitOfWork.Repository<SolicitudServicio>().Update(solicitud);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Precio acordado establecido correctamente.");
        }

        public async Task<Result> ConfirmarFinalizacionAsync(Guid solicitudId, Guid clienteId,
            ConfirmarFinalizacionRequest request, CancellationToken cancellationToken = default)
        {
            var solicitud = await _unitOfWork.Repository<SolicitudServicio>().GetByIdAsync(solicitudId, cancellationToken);
            if (solicitud is null || !solicitud.Activo) return Result.Fail("Solicitud no encontrada.");
            if (solicitud.ClienteId != clienteId)
                return Result.Fail("Solo el cliente de la solicitud puede confirmar la finalización.");
            if (solicitud.Estado != EstadoSolicitudServicio.PendienteConfirmacionCliente)
                return Result.Fail("La solicitud no está pendiente de confirmación del cliente.");

            if (request.Confirmado)
            {
                if (!solicitud.PrecioAcordado.HasValue || solicitud.PrecioAcordado <= 0)
                    return Result.Fail("No se puede confirmar sin un precio acordado válido.");
                solicitud.Estado = EstadoSolicitudServicio.PendientePago;
                solicitud.FechaConfirmacionCliente = DateTime.UtcNow;
                solicitud.MotivoRechazoFinalizacion = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.MotivoRechazo))
                    return Result.Fail("Debes indicar por qué el servicio todavía no ha finalizado.");
                if (request.MotivoRechazo.Trim().Length > 1000)
                    return Result.Fail("El motivo no puede exceder 1000 caracteres.");
                solicitud.Estado = EstadoSolicitudServicio.Aceptada;
                solicitud.FechaRechazoFinalizacionCliente = DateTime.UtcNow;
                solicitud.MotivoRechazoFinalizacion = request.MotivoRechazo.Trim();
            }

            solicitud.FechaModificacion = DateTime.UtcNow;
            _unitOfWork.Repository<SolicitudServicio>().Update(solicitud);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(request.Confirmado
                ? "Finalización confirmada. La solicitud está lista para el pago."
                : "Finalización rechazada. La solicitud volvió a estado aceptada.");
        }

        private static CambiarEstadoSolicitudResponse CrearRespuestaCambioEstado(
            SolicitudServicio solicitud,
            Guid? conversacionId = null,
            string? mensajeAutomatico = null) => new()
        {
            SolicitudId = solicitud.Id,
            ClienteId = solicitud.ClienteId,
            EstadoId = (int)solicitud.Estado,
            Estado = solicitud.Estado.ToString(),
            ConversacionId = conversacionId,
            MensajeAutomatico = mensajeAutomatico
        };

        private static string CrearMensajeInicial(SolicitudServicio solicitud)
        {
            var saludo = $"¡Hola, {solicitud.Cliente.Nombre}! He aceptado tu solicitud para " +
                         $"\"{solicitud.Servicio.Titulo}\".";

            return solicitud.FechaDeseada.HasValue
                ? $"{saludo} Quisiera confirmar si la fecha propuesta, " +
                  $"{solicitud.FechaDeseada.Value:dd/MM/yyyy}, todavía te funciona. " +
                  "Si necesitas cambiarla, podemos coordinar una nueva fecha por aquí."
                : $"{saludo} Quisiera coordinar contigo la fecha para realizar el servicio. " +
                  "¿Qué día y hora te funcionan mejor?";
        }

        public async Task<Result<SolicitudServicioDetalleDto>> ObtenerDetalleAsync(Guid solicitudId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            var solicitudes = await _unitOfWork
            .Repository<SolicitudServicio>()
            .GetAsync(
                x => x.Id == solicitudId && x.Activo,
            null,
            cancellationToken,
            x => x.Cliente,
            x => x.Cliente.Perfil!,
            x => x.Servicio,
            x => x.Servicio.Usuario,
            x => x.Servicio.Usuario.Perfil!,
            x => x.Resenas,
            x => x.Pago!,
            x => x.Servicio.CategoriaServicio);

            var solicitud = solicitudes.FirstOrDefault();

            if (solicitud is null)
                return Result<SolicitudServicioDetalleDto>.Fail("Solicitud no encontrada.");

            var proveedorId = solicitud.Servicio.UsuarioId;

            var usuarioTieneAcceso =
                solicitud.ClienteId == usuarioId ||
                proveedorId == usuarioId;

            if (!usuarioTieneAcceso)
                return Result<SolicitudServicioDetalleDto>.Fail("No tienes permiso para ver esta solicitud.");

            var dto = new SolicitudServicioDetalleDto
            {
                Id = solicitud.Id,

                ClienteId = solicitud.ClienteId,
                Cliente = $"{solicitud.Cliente.Nombre} {solicitud.Cliente.Apellido}",
                CalificacionCliente = solicitud.Cliente.Perfil?.CalificacionPromedio ?? 0,
                TotalResenasCliente = solicitud.Cliente.Perfil?.TotalResenas ?? 0,

                ServicioId = solicitud.ServicioId,
                TituloServicio = solicitud.Servicio.Titulo,
                DescripcionServicio = solicitud.Servicio.Descripcion,
                Categoria = solicitud.Servicio.CategoriaServicio.Nombre,

                ProveedorId = proveedorId,
                Proveedor = $"{solicitud.Servicio.Usuario.Nombre} {solicitud.Servicio.Usuario.Apellido}",
                CalificacionProveedor = solicitud.Servicio.Usuario.Perfil?.CalificacionPromedio ?? 0,
                TotalResenasProveedor = solicitud.Servicio.Usuario.Perfil?.TotalResenas ?? 0,

                DescripcionSolicitud = solicitud.DescripcionSolicitud,
                FechaDeseada = solicitud.FechaDeseada,
                PrecioAcordado = ObtenerPrecioVisible(
                    solicitud.PrecioAcordado,
                    solicitud.Servicio,
                    solicitud.Estado,
                    solicitud.Pago),
                FechaReporteFinalizacionProveedor = solicitud.FechaReporteFinalizacionProveedor,
                FechaConfirmacionCliente = solicitud.FechaConfirmacionCliente,
                FechaRechazoFinalizacionCliente = solicitud.FechaRechazoFinalizacionCliente,
                MotivoRechazoFinalizacion = solicitud.MotivoRechazoFinalizacion,

                EstadoId = (int)solicitud.Estado,
                Estado = solicitud.Estado.ToString(),

                FechaCreacion = solicitud.FechaCreacion,
                FechaModificacion = solicitud.FechaModificacion,

                AvancePorcentaje = ObtenerAvancePorEstado(solicitud.Estado),
                YaCalifico = solicitud.Resenas.Any(x => x.EvaluadorId == usuarioId && x.Activo),
                PuedeCalificar = solicitud.Estado == EstadoSolicitudServicio.Completada &&
                    !solicitud.Resenas.Any(x => x.EvaluadorId == usuarioId && x.Activo),
                PuedeConfirmarFinalizacion = solicitud.ClienteId == usuarioId &&
                    solicitud.Estado == EstadoSolicitudServicio.PendienteConfirmacionCliente,
                PuedePagar = solicitud.ClienteId == usuarioId &&
                    solicitud.Estado == EstadoSolicitudServicio.PendientePago
            };

            return Result<SolicitudServicioDetalleDto>.Ok(dto);
        }

        public async Task<Result<PagedResponse<SolicitudesClienteResponse>>> ObtenerMisSolicitudesAsync(Guid clienteId, SolicitudServicioFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            var texto = filtro.Busqueda?.Trim().ToLower();
            var ordenarPor = filtro.OrdenarPor?.Trim().ToLower();

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x => x.ClienteId == clienteId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query
                        .WithCondition(
                            x => x.Servicio.Titulo.ToLower().Contains(texto!) ||
                                 x.Servicio.Descripcion.ToLower().Contains(texto!) ||
                                 x.Servicio.Usuario.Nombre.ToLower().Contains(texto!) ||
                                 x.Servicio.Usuario.Apellido.ToLower().Contains(texto!) ||
                                 x.Servicio.CategoriaServicio.Nombre.ToLower().Contains(texto!),
                            !string.IsNullOrWhiteSpace(texto))

                        .WithCondition(
                            x => x.Estado == filtro.Estado,
                            filtro.Estado > 0)

                         .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "antiguas",
                            "asc")

                        .WithOrderBy(
                            x => x.PrecioAcordado,
                            ordenarPor == "precio_mayor",
                            "desc")

                        .WithOrderBy(
                            x => x.PrecioAcordado,
                            ordenarPor == "precio_menor",
                            "asc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "recientes" || string.IsNullOrWhiteSpace(ordenarPor),
                            "desc")
                        ,
                    includes:
                    [
                        x => x.Servicio,
                        x => x.Servicio.Usuario,
                        x => x.Servicio.Usuario.Perfil!,
                        x => x.Resenas,
                        x => x.Pago!,
                        x => x.Servicio.CategoriaServicio
                    ]);

            var totalRecords = solicitudes.Count;

            var solicitudesResumen = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetAsync(
                    x => x.ClienteId == clienteId && x.Activo,
                    cancellationToken: cancellationToken);

            var resumenCompleto = new
            {
                TotalSolicitudes = solicitudesResumen.Count,
                Pendientes = solicitudesResumen.Count(x => x.Estado == EstadoSolicitudServicio.Pendiente),
                Activas = solicitudesResumen.Count(x => x.Estado == EstadoSolicitudServicio.Aceptada),
                Completadas = solicitudesResumen.Count(x => x.Estado == EstadoSolicitudServicio.Completada),
                Canceladas = solicitudesResumen.Count(x => x.Estado == EstadoSolicitudServicio.Cancelada)
            };              

            var solicitudesPaginadas = solicitudes
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToList();

            var response = new SolicitudesClienteResponse
            {
                TotalSolicitudes = resumenCompleto.TotalSolicitudes,
                Pendientes = resumenCompleto.Pendientes,
                Activas = resumenCompleto.Activas,
                Completadas = resumenCompleto.Completadas,
                Canceladas = resumenCompleto.Canceladas,

                Solicitudes = solicitudesPaginadas.Select(x => new SolicitudServicioListadoDto
                {
                    Id = x.Id,
                    ServicioId = x.ServicioId,
                    TituloServicio = x.Servicio.Titulo,
                    Categoria = x.Servicio.CategoriaServicio.Nombre,
                    ProveedorId = x.Servicio.UsuarioId,
                    Proveedor = $"{x.Servicio.Usuario.Nombre} {x.Servicio.Usuario.Apellido}",
                    CalificacionProveedor = x.Servicio.Usuario.Perfil?.CalificacionPromedio ?? 0,
                    TotalResenasProveedor = x.Servicio.Usuario.Perfil?.TotalResenas ?? 0,
                    DescripcionSolicitud = x.DescripcionSolicitud,
                    FechaDeseada = x.FechaDeseada,
                    PrecioAcordado = ObtenerPrecioVisible(
                        x.PrecioAcordado,
                        x.Servicio,
                        x.Estado,
                        x.Pago),
                    EstadoId = (int)x.Estado,
                    Estado = x.Estado.ToString(),
                    FechaCreacion = x.FechaCreacion,
                    AvancePorcentaje = ObtenerAvancePorEstado(x.Estado),
                    YaCalifico = x.Resenas.Any(r => r.EvaluadorId == clienteId && r.Activo),
                    PuedeCalificar = x.Estado == EstadoSolicitudServicio.Completada &&
                        !x.Resenas.Any(r => r.EvaluadorId == clienteId && r.Activo)
                }).ToList()
            };

            var pagedResponse = PagedResponse<SolicitudesClienteResponse>.Create(
                response,
                filtro.Page,
                filtro.PageSize,
                totalRecords);

            return Result<PagedResponse<SolicitudesClienteResponse>>.Ok(pagedResponse);
        }

        public async Task<Result<PagedResponse<SolicitudesProveedorResponse>>> ObtenerSolicitudesRecibidasAsync(Guid proveedorId, SolicitudServicioFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            if (proveedorId == Guid.Empty)
            {
                return Result<PagedResponse<SolicitudesProveedorResponse>>.Fail(
                    "El proveedor autenticado no es válido.");
            }

            var texto = filtro.Busqueda?.Trim().ToLower();

            var ordenarPor = filtro.OrdenarPor?
                .Trim()
                .ToLower();

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x =>
                        x.Servicio.UsuarioId == proveedorId &&
                        x.Activo,

                    orderBy: null,

                    cancellationToken: cancellationToken,

                    queryModifier: query => query

                        .WithCondition(
                            x =>
                                x.Servicio.Titulo
                                    .ToLower()
                                    .Contains(texto!) ||

                                x.DescripcionSolicitud
                                    .ToLower()
                                    .Contains(texto!) ||

                                x.Cliente.Nombre
                                    .ToLower()
                                    .Contains(texto!) ||

                                x.Cliente.Apellido
                                    .ToLower()
                                    .Contains(texto!) ||

                                x.Servicio.CategoriaServicio.Nombre
                                    .ToLower()
                                    .Contains(texto!),

                            !string.IsNullOrWhiteSpace(texto))

                        .WithCondition(
                            x => x.Estado == filtro.Estado,
                            filtro.Estado > 0)

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "antiguas",
                            "asc")

                        .WithOrderBy(
                            x => x.PrecioAcordado,
                            ordenarPor == "precio_mayor",
                            "desc")

                        .WithOrderBy(
                            x => x.PrecioAcordado,
                            ordenarPor == "precio_menor",
                            "asc")

                        .WithOrderBy(
                            x => x.FechaCreacion,
                            ordenarPor == "recientes" ||
                            string.IsNullOrWhiteSpace(ordenarPor),
                            "desc"),

                    includes:
                    [
                        x => x.Cliente,
        x => x.Cliente.Perfil!,
        x => x.Servicio,
        x => x.Resenas,
        x => x.Pago!,
        x => x.Servicio.CategoriaServicio
                    ]);

            var totalRecords = solicitudes.Count;

            /*
             * Para las cards/resumen no usamos los filtros
             * de búsqueda ni estado.
             */
            var solicitudesResumen = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetAsync(
                    x =>
                        x.Servicio.UsuarioId == proveedorId &&
                        x.Activo,

                    cancellationToken: cancellationToken);

            var resumen = new
            {
                TotalSolicitudes =
                    solicitudesResumen.Count,

                Pendientes =
                    solicitudesResumen.Count(x =>
                        x.Estado ==
                        EstadoSolicitudServicio.Pendiente),

                Aceptadas =
                    solicitudesResumen.Count(x =>
                        x.Estado ==
                        EstadoSolicitudServicio.Aceptada),

                Completadas =
                    solicitudesResumen.Count(x =>
                        x.Estado ==
                        EstadoSolicitudServicio.Completada),

                Canceladas =
                    solicitudesResumen.Count(x =>
                        x.Estado ==
                        EstadoSolicitudServicio.Cancelada)
            };

            var solicitudesPaginadas = solicitudes
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToList();

            var response =
                new SolicitudesProveedorResponse
                {
                    TotalSolicitudes =
                        resumen.TotalSolicitudes,

                    Pendientes =
                        resumen.Pendientes,

                    Aceptadas =
                        resumen.Aceptadas,

                    Completadas =
                        resumen.Completadas,

                    Canceladas =
                        resumen.Canceladas,

                    Solicitudes = solicitudesPaginadas
                        .Select(x =>
                            new SolicitudProveedorListadoDto
                            {
                                Id = x.Id,

                                ServicioId =
                                    x.ServicioId,

                                TituloServicio =
                                    x.Servicio.Titulo,

                                Categoria =
                                    x.Servicio
                                        .CategoriaServicio
                                        .Nombre,

                                ClienteId =
                                    x.ClienteId,

                                Cliente =
                                    $"{x.Cliente.Nombre} {x.Cliente.Apellido}",

                                CalificacionCliente =
                                    x.Cliente.Perfil?.CalificacionPromedio ?? 0,

                                TotalResenasCliente =
                                    x.Cliente.Perfil?.TotalResenas ?? 0,

                                DescripcionSolicitud =
                                    x.DescripcionSolicitud,

                                FechaDeseada =
                                    x.FechaDeseada,

                                PrecioAcordado =
                                    ObtenerPrecioVisible(
                                        x.PrecioAcordado,
                                        x.Servicio,
                                        x.Estado,
                                        x.Pago),

                                EstadoId =
                                    (int)x.Estado,

                                Estado =
                                    x.Estado.ToString(),

                                FechaCreacion =
                                    x.FechaCreacion,

                                AvancePorcentaje =
                                    ObtenerAvancePorEstado(
                                        x.Estado),

                                YaCalifico = x.Resenas.Any(r =>
                                    r.EvaluadorId == proveedorId && r.Activo),

                                PuedeCalificar =
                                    x.Estado == EstadoSolicitudServicio.Completada &&
                                    !x.Resenas.Any(r =>
                                        r.EvaluadorId == proveedorId && r.Activo)
                            })
                        .ToList()
                };

            var pagedResponse =
                PagedResponse<SolicitudesProveedorResponse>.Create(
                    response,
                    filtro.Page,
                    filtro.PageSize,
                    totalRecords);

            return Result<PagedResponse<SolicitudesProveedorResponse>>
                .Ok(pagedResponse);
        }

        private static int ObtenerAvancePorEstado(EstadoSolicitudServicio estado)
        {
            return estado switch
            {
                EstadoSolicitudServicio.Pendiente => 20,
                EstadoSolicitudServicio.Aceptada => 60,
                EstadoSolicitudServicio.PendienteConfirmacionCliente => 80,
                EstadoSolicitudServicio.PendientePago => 90,
                EstadoSolicitudServicio.Completada => 100,
                EstadoSolicitudServicio.Cancelada => 0,
                EstadoSolicitudServicio.Rechazada => 0,
                _ => 0
            };
        }

        private static decimal? ObtenerPrecioVisible(
            decimal? precioAcordado,
            Servicio servicio,
            EstadoSolicitudServicio? estado = null,
            Pago? pago = null)
        {
            if (precioAcordado.HasValue)
                return precioAcordado;

            if (servicio.PrecioNegociable &&
                estado == EstadoSolicitudServicio.Completada &&
                pago is { Estado: EstadoPago.Pagado, MontoServicio: > 0 })
            {
                return decimal.Round(
                    pago.MontoServicio,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            if (servicio.PrecioNegociable ||
                !servicio.PrecioBase.HasValue ||
                servicio.PrecioBase.Value <= 0)
            {
                return null;
            }

            return decimal.Round(
                servicio.PrecioBase.Value,
                2,
                MidpointRounding.AwayFromZero);
        }

        private static bool PuedeCambiarEstado(EstadoSolicitudServicio actual, EstadoSolicitudServicio nuevo)
        {
            return actual switch
            {
                EstadoSolicitudServicio.Pendiente =>
                    nuevo ==
                        EstadoSolicitudServicio.Aceptada ||
                    nuevo ==
                        EstadoSolicitudServicio.Rechazada,

                EstadoSolicitudServicio.Aceptada =>
                    nuevo ==
                        EstadoSolicitudServicio.PendienteConfirmacionCliente ||
                    nuevo ==
                        EstadoSolicitudServicio.Cancelada,

                EstadoSolicitudServicio.PendienteConfirmacionCliente => false,

                EstadoSolicitudServicio.PendientePago => false,

                EstadoSolicitudServicio.Completada =>
                    false,

                EstadoSolicitudServicio.Cancelada =>
                    false,

                EstadoSolicitudServicio.Rechazada =>
                    false,

                _ => false
            };
        }

        
    }
}
