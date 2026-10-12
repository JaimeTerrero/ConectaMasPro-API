using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.Services
{
    public class ProgresoProveedorService : IProgresoProveedorService
    {
        private const int MetaServiciosMensual = 10;

        private readonly IUnitOfWork _unitOfWork;

        public ProgresoProveedorService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> RegistrarServicioCompletadoAsync(SolicitudServicio solicitud, CancellationToken cancellationToken = default)
        {
            if (solicitud is null)
            {
                return Result.Fail(
                    "La solicitud de servicio no es válida.");
            }

            if (solicitud.Servicio is null)
            {
                return Result.Fail(
                    "No fue posible identificar el servicio asociado a la solicitud.");
            }

            var proveedorId = solicitud.Servicio.UsuarioId;

            if (proveedorId == Guid.Empty)
            {
                return Result.Fail(
                    "No fue posible identificar al proveedor del servicio.");
            }

            /*
             * Evitamos que una misma solicitud
             * incremente el progreso dos veces.
             */
            var movimientosExistentes = await _unitOfWork
                .Repository<MovimientoProgresoProveedor>()
                .FindAsync(
                    x =>
                        x.SolicitudServicioId == solicitud.Id &&
                        x.Activo,
                    cancellationToken);

            if (movimientosExistentes.Any())
            {
                return Result.Ok();
            }

            /*
             * Buscamos el progreso histórico
             * del proveedor.
             */
            var progresos = await _unitOfWork
                .Repository<ProveedorProgreso>()
                .FindAsync(
                    x =>
                        x.UsuarioId == proveedorId &&
                        x.Activo,
                    cancellationToken);

            var proveedorProgreso =
                progresos.FirstOrDefault();

            var progresoExistia =
                proveedorProgreso is not null;

            /*
             * Si nunca ha completado servicios,
             * creamos su progreso.
             */
            if (proveedorProgreso is null)
            {
                proveedorProgreso = new ProveedorProgreso
                {
                    UsuarioId = proveedorId,
                    ServiciosCompletados = 0,
                    Nivel = NivelProveedor.Nuevo
                };

                await _unitOfWork
                    .Repository<ProveedorProgreso>()
                    .AddAsync(
                        proveedorProgreso,
                        cancellationToken);
            }

            var serviciosAnteriores =
                proveedorProgreso.ServiciosCompletados;

            /*
             * Incrementamos el progreso histórico.
             */
            proveedorProgreso.ServiciosCompletados++;

            proveedorProgreso.Nivel =
                CalcularNivel(
                    proveedorProgreso.ServiciosCompletados);

            proveedorProgreso.FechaModificacion =
                DateTime.UtcNow;

            /*
             * Si ya existía en BD, debemos
             * indicarle al repositorio que se actualizó.
             */
            if (progresoExistia)
            {
                _unitOfWork
                    .Repository<ProveedorProgreso>()
                    .Update(proveedorProgreso);
            }

            /*
             * Trabajamos ahora con la meta
             * correspondiente al mes actual.
             */
            var fechaActual = DateTime.UtcNow;

            var metas = await _unitOfWork
                .Repository<ProveedorMetaMensual>()
                .FindAsync(
                    x =>
                        x.ProveedorProgresoId == proveedorProgreso.Id &&
                        x.Anio == fechaActual.Year &&
                        x.Mes == fechaActual.Month &&
                        x.Activo,
                    cancellationToken);

            var metaMensual =
                metas.FirstOrDefault();

            var metaExistia =
                metaMensual is not null;

            /*
             * Si es el primer servicio completado
             * del proveedor durante este mes,
             * creamos automáticamente su meta.
             */
            if (metaMensual is null)
            {
                metaMensual = new ProveedorMetaMensual
                {
                    ProveedorProgresoId =
                        proveedorProgreso.Id,

                    Anio =
                        fechaActual.Year,

                    Mes =
                        fechaActual.Month,

                    MetaServicios =
                        MetaServiciosMensual,

                    ServiciosCompletados = 0,

                    MetaCumplida = false
                };

                await _unitOfWork
                    .Repository<ProveedorMetaMensual>()
                    .AddAsync(
                        metaMensual,
                        cancellationToken);
            }

            /*
             * Incrementamos el progreso mensual.
             */
            metaMensual.ServiciosCompletados++;

            /*
             * Solamente registramos la fecha
             * la primera vez que alcanza la meta.
             */
            if (!metaMensual.MetaCumplida &&
                metaMensual.ServiciosCompletados >=
                metaMensual.MetaServicios)
            {
                metaMensual.MetaCumplida = true;

                metaMensual.FechaCumplimiento =
                    DateTime.UtcNow;
            }

            metaMensual.FechaModificacion =
                DateTime.UtcNow;

            if (metaExistia)
            {
                _unitOfWork
                    .Repository<ProveedorMetaMensual>()
                    .Update(metaMensual);
            }

            /*
             * Finalmente registramos el movimiento
             * que demuestra qué solicitud incrementó
             * el progreso.
             */
            var movimiento =
                new MovimientoProgresoProveedor
                {
                    ProveedorProgresoId =
                        proveedorProgreso.Id,

                    SolicitudServicioId =
                        solicitud.Id,

                    ServiciosAnteriores =
                        serviciosAnteriores,

                    ServiciosPosteriores =
                        proveedorProgreso.ServiciosCompletados,

                    Concepto =
                        "Servicio completado"
                };

            await _unitOfWork
                .Repository<MovimientoProgresoProveedor>()
                .AddAsync(
                    movimiento,
                    cancellationToken);

            /*
             * NO hacemos SaveChanges aquí.
             * SolicitudServicioService guardará
             * toda la operación junta.
             */
            return Result.Ok();
        }

        private static NivelProveedor CalcularNivel(int serviciosCompletados)
        {
            return serviciosCompletados switch
            {
                >= 20 => NivelProveedor.Referente,
                >= 10 => NivelProveedor.Destacado,
                >= 5 => NivelProveedor.Activo,
                _ => NivelProveedor.Nuevo
            };
        }
    }
}
