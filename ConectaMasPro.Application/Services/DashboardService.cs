using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Common.Extensions;
using ConectaMasPro.Application.DTOs.Dashboard;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ClienteDashboardDto>> ObtenerDashboardClienteAsync(Guid clienteId,
            CancellationToken cancellationToken = default)
        {
            var perfilesCliente = await _unitOfWork.Repository<PerfilUsuario>()
                .FindAsync(x => x.UsuarioId == clienteId && x.Activo, cancellationToken);
            var perfilCliente = perfilesCliente.FirstOrDefault();

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x => x.ClienteId == clienteId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query.WithOrderBy(x => x.FechaCreacion, true, "desc"),
                    includes:
                    [
                        x => x.Servicio,
                    x => x.Servicio.Usuario,
                    x => x.Servicio.CategoriaServicio,
                    x => x.Pago
                    ]);

            var solicitudesActivas = solicitudes.Count(x =>
                x.Estado == EstadoSolicitudServicio.Pendiente ||
                x.Estado == EstadoSolicitudServicio.Aceptada);

            var serviciosCompletados = solicitudes.Count(x =>
                x.Estado == EstadoSolicitudServicio.Completada);

            var pagosRealizados = solicitudes
                .Where(x => x.Pago is not null && x.Pago.Estado == EstadoPago.Pagado)
                .Sum(x => x.Pago!.Monto);

            var solicitudesRecientes = solicitudes
                .OrderByDescending(x => x.FechaCreacion)
                .Take(3)
                .Select(x => new SolicitudRecienteDto
                {
                    Id = x.Id,
                    TituloServicio = x.Servicio.Titulo,
                    Proveedor = $"{x.Servicio.Usuario.Nombre} {x.Servicio.Usuario.Apellido}",
                    FechaCreacion = x.FechaCreacion,
                    EstadoId = (int)x.Estado,
                    Estado = x.Estado.ToString()
                })
                .ToList();

            var proveedores = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query.WithOrderBy(x => x.FechaCreacion, true, "desc"),
                    includes:
                    [
                        x => x.Usuario,
                    x => x.Usuario.Perfil!,
                    x => x.CategoriaServicio
                    ]);

            var proveedoresRecomendados = proveedores
                .GroupBy(x => x.UsuarioId)
                .Select(g =>
                {
                    var servicio = g.First();

                    return new ProveedorRecomendadoDto
                    {
                        ProveedorId = servicio.UsuarioId,
                        Nombre = $"{servicio.Usuario.Nombre} {servicio.Usuario.Apellido}",
                        Categoria = servicio.CategoriaServicio.Nombre,
                        CalificacionPromedio = servicio.Usuario.Perfil?.CalificacionPromedio ?? 0
                    };
                })
                .Take(3)
                .ToList();

            var dashboard = new ClienteDashboardDto
            {
                Resumen = new ClienteDashboardResumenDto
                {
                    SolicitudesActivas = solicitudesActivas,
                    ServiciosCompletados = serviciosCompletados,
                    PagosRealizados = pagosRealizados,
                    CalificacionPromedio = perfilCliente?.CalificacionPromedio ?? 0
                },
                SolicitudesRecientes = solicitudesRecientes,
                ProveedoresRecomendados = proveedoresRecomendados
            };

            return Result<ClienteDashboardDto>.Ok(dashboard);
        }

        public async Task<Result<ProveedorDashboardDto>> ObtenerDashboardProveedorAsync(
    Guid proveedorId,
    CancellationToken cancellationToken = default)
        {
            var perfilesProveedor = await _unitOfWork.Repository<PerfilUsuario>()
                .FindAsync(x => x.UsuarioId == proveedorId && x.Activo, cancellationToken);
            var perfilProveedor = perfilesProveedor.FirstOrDefault();

            var servicios = await _unitOfWork
                .Repository<Servicio>()
                .GetWithQueryAsync(
                    predicate: x => x.UsuarioId == proveedorId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query.WithOrderBy(x => x.FechaCreacion, true, "desc"),
                    includes:
                    [
                        x => x.CategoriaServicio
                    ]);

            var solicitudes = await _unitOfWork
                .Repository<SolicitudServicio>()
                .GetWithQueryAsync(
                    predicate: x => x.Servicio.UsuarioId == proveedorId && x.Activo,
                    orderBy: null,
                    cancellationToken: cancellationToken,
                    queryModifier: query => query.WithOrderBy(x => x.FechaCreacion, true, "desc"),
                    includes:
                    [
                        x => x.Cliente,
                x => x.Servicio,
                x => x.Servicio.CategoriaServicio,
                x => x.Pago
                    ]);

            var solicitudesRecibidas = solicitudes
                .Take(5)
                .Select(x => new SolicitudRecibidaDto
                {
                    Id = x.Id,
                    TituloServicio = x.Servicio.Titulo,
                    Cliente = $"{x.Cliente.Nombre} {x.Cliente.Apellido}",
                    FechaCreacion = x.FechaCreacion,
                    FechaDeseada = x.FechaDeseada,
                    PrecioAcordado = x.PrecioAcordado,
                    EstadoId = (int)x.Estado,
                    Estado = x.Estado.ToString()
                })
                .ToList();

            var misServiciosPublicados = servicios
                .Take(5)
                .Select(x => new ServicioProveedorDto
                {
                    Id = x.Id,
                    TituloServicio = x.Titulo,
                    Categoria = x.CategoriaServicio.Nombre,
                    PrecioBase = x.PrecioBase,
                    PrecioNegociable = x.PrecioNegociable,
                    FechaCreacion = x.FechaCreacion
                })
                .ToList();

            var ingresosEstimados = solicitudes
                .Where(x =>
                    x.Estado == EstadoSolicitudServicio.Aceptada ||
                    x.Estado == EstadoSolicitudServicio.Completada)
                .Sum(x => x.PrecioAcordado ?? 0);

            var dashboard = new ProveedorDashboardDto
            {
                Resumen = new ProveedorDashboardResumenDto
                {
                    SolicitudesRecibidas = solicitudes.Count,
                    ServiciosActivos = servicios.Count,
                    IngresosEstimados = ingresosEstimados,
                    CalificacionPromedio = perfilProveedor?.CalificacionPromedio ?? 0
                },
                SolicitudesRecibidas = solicitudesRecibidas,
                MisServiciosPublicados = misServiciosPublicados
            };

            return Result<ProveedorDashboardDto>.Ok(dashboard);
        }
    }
}
