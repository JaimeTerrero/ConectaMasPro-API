using ConectaMasPro.Application.Common;
using ConectaMasPro.Application.DTOs.Pagos;
using ConectaMasPro.Application.Interfaces.Repositories;
using ConectaMasPro.Application.Interfaces.Services;
using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Domain.Enums;
using ConectaMasPro.Application.Common.Pagination;

namespace ConectaMasPro.Application.Services;

public class PagoService : IPagoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasarelaPagoService _pasarela;
    private readonly IRecompensaService _recompensaService;
    private readonly IProgresoProveedorService _progresoProveedorService;

    public PagoService(IUnitOfWork unitOfWork, IPasarelaPagoService pasarela,
        IRecompensaService recompensaService, IProgresoProveedorService progresoProveedorService)
    {
        _unitOfWork = unitOfWork;
        _pasarela = pasarela;
        _recompensaService = recompensaService;
        _progresoProveedorService = progresoProveedorService;
    }

    public Result<ConfiguracionPublicaStripeDto> ObtenerConfiguracionPublica()
    {
        if (string.IsNullOrWhiteSpace(_pasarela.ClavePublicable) ||
            !_pasarela.ClavePublicable.StartsWith("pk_test_", StringComparison.Ordinal))
            return Result<ConfiguracionPublicaStripeDto>.Fail(
                "Configura una clave publicable Stripe de prueba válida.");
        return Result<ConfiguracionPublicaStripeDto>.Ok(new ConfiguracionPublicaStripeDto
        {
            ClavePublicable = _pasarela.ClavePublicable,
            Moneda = _pasarela.Moneda,
            PorcentajeComision = _pasarela.PorcentajeComision
        });
    }

    public async Task<Result<ResumenPagoDto>> ObtenerResumenAsync(Guid solicitudId, Guid clienteId, CancellationToken ct = default)
    {
        var solicitud = await ObtenerSolicitudAsync(solicitudId, ct);
        if (solicitud is null || !solicitud.Activo) return Result<ResumenPagoDto>.Fail("La solicitud no existe.");
        if (solicitud.ClienteId != clienteId) return Result<ResumenPagoDto>.Fail("Solo el cliente de la solicitud puede consultar el pago.");
        if (!solicitud.PrecioAcordado.HasValue || solicitud.PrecioAcordado <= 0) return Result<ResumenPagoDto>.Fail("La solicitud no tiene un precio acordado válido.");
        return Result<ResumenPagoDto>.Ok(CrearResumen(solicitud));
    }

    public async Task<Result<IntencionPagoDto>> CrearIntencionAsync(Guid solicitudId, Guid clienteId, CancellationToken ct = default)
    {
        var solicitud = await ObtenerSolicitudAsync(solicitudId, ct);
        if (solicitud is null || !solicitud.Activo) return Result<IntencionPagoDto>.Fail("La solicitud no existe.");
        if (solicitud.ClienteId != clienteId) return Result<IntencionPagoDto>.Fail("Solo el cliente de la solicitud puede iniciar el pago.");
        if (solicitud.Estado != EstadoSolicitudServicio.PendientePago) return Result<IntencionPagoDto>.Fail("El cliente debe confirmar la finalización antes de pagar.");
        if (!solicitud.PrecioAcordado.HasValue || solicitud.PrecioAcordado <= 0) return Result<IntencionPagoDto>.Fail("La solicitud no tiene un precio acordado válido.");

        var resumen = CrearResumen(solicitud);
        var pagoExistente = (await _unitOfWork.Repository<Pago>().FindAsync(
            x => x.SolicitudServicioId == solicitudId && x.Activo, ct)).FirstOrDefault();
        try
        {
            if (pagoExistente?.ReferenciaTransaccion is not null)
            {
                var existente = await _pasarela.ObtenerIntencionAsync(pagoExistente.ReferenciaTransaccion, ct);
                return Result<IntencionPagoDto>.Ok(MapIntencion(pagoExistente, resumen, existente), "Se reutilizó la intención de pago existente.");
            }

            var montoMenorUnidad = decimal.ToInt64(decimal.Round(resumen.TotalPagar * 100m, 0, MidpointRounding.AwayFromZero));
            var intencion = await _pasarela.CrearIntencionAsync(montoMenorUnidad,
                $"Servicio: {solicitud.Servicio.Titulo}",
                new Dictionary<string, string>
                {
                    ["solicitudServicioId"] = solicitud.Id.ToString(),
                    ["clienteId"] = solicitud.ClienteId.ToString(),
                    ["proveedorId"] = solicitud.Servicio.UsuarioId.ToString()
                }, $"solicitud-{solicitud.Id}", ct);

            var pago = pagoExistente ?? new Pago { SolicitudServicioId = solicitud.Id };
            pago.MontoServicio = resumen.MontoServicio;
            pago.Comision = resumen.Comision;
            pago.PorcentajeComision = resumen.PorcentajeComision;
            pago.Monto = resumen.TotalPagar;
            pago.Moneda = resumen.Moneda;
            pago.ReferenciaTransaccion = intencion.Id;
            pago.EstadoPasarela = intencion.Estado;
            pago.Estado = EstadoPago.Pendiente;

            if (pagoExistente is null) await _unitOfWork.Repository<Pago>().AddAsync(pago, ct);
            else _unitOfWork.Repository<Pago>().Update(pago);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<IntencionPagoDto>.Ok(MapIntencion(pago, resumen, intencion), "Intención de pago creada correctamente.");
        }
        catch (Exception ex)
        {
            return Result<IntencionPagoDto>.Fail($"Stripe no pudo crear la intención de pago: {ex.Message}");
        }
    }

    public async Task<Result<PagoDetalleDto>> SincronizarAsync(Guid pagoId, Guid clienteId, CancellationToken ct = default)
    {
        var pago = (await _unitOfWork.Repository<Pago>().GetTrackedAsync(x => x.Id == pagoId && x.Activo,
            null, ct, x => x.SolicitudServicio, x => x.SolicitudServicio.Servicio)).FirstOrDefault();
        if (pago is null) return Result<PagoDetalleDto>.Fail("El pago no existe.");
        if (pago.SolicitudServicio.ClienteId != clienteId) return Result<PagoDetalleDto>.Fail("Solo el cliente puede sincronizar este pago.");
        if (string.IsNullOrWhiteSpace(pago.ReferenciaTransaccion)) return Result<PagoDetalleDto>.Fail("El pago no tiene una referencia de Stripe.");
        try
        {
            var intencion = await _pasarela.ObtenerIntencionAsync(pago.ReferenciaTransaccion, ct);
            var resultado = await AplicarEstadoPasarelaAsync(pago, intencion.Estado, intencion.UltimoError, ct);
            return resultado.Success ? Result<PagoDetalleDto>.Ok(MapPago(pago), "Estado sincronizado con Stripe.")
                : Result<PagoDetalleDto>.Fail(resultado.Message ?? "No fue posible actualizar el pago.");
        }
        catch (Exception ex)
        {
            return Result<PagoDetalleDto>.Fail($"No fue posible consultar Stripe: {ex.Message}");
        }
    }

    public async Task<Result<PagedResponse<GananciasProveedorResponse>>> ObtenerGananciasProveedorAsync(
        Guid proveedorId, GananciasProveedorFiltroRequest filtro, CancellationToken ct = default)
    {
        if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde > filtro.Hasta)
            return Result<PagedResponse<GananciasProveedorResponse>>.Fail(
                "La fecha inicial no puede ser posterior a la fecha final.");

        var desde = filtro.Desde?.ToUniversalTime();
        var hasta = filtro.Hasta?.ToUniversalTime();
        var pagos = await _unitOfWork.Repository<Pago>().GetAsync(
            x => x.Activo && x.Estado == EstadoPago.Pagado && x.FechaPago.HasValue &&
                 x.SolicitudServicio.Servicio.UsuarioId == proveedorId &&
                 (!desde.HasValue || x.FechaPago >= desde.Value) &&
                 (!hasta.HasValue || x.FechaPago <= hasta.Value),
            q => q.OrderByDescending(x => x.FechaPago), ct,
            x => x.SolicitudServicio.Cliente,
            x => x.SolicitudServicio.Servicio);

        var movimientos = pagos
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .Select(x => new GananciaProveedorDto
            {
                PagoId = x.Id,
                SolicitudServicioId = x.SolicitudServicioId,
                Servicio = x.SolicitudServicio.Servicio.Titulo,
                Cliente = $"{x.SolicitudServicio.Cliente.Nombre} {x.SolicitudServicio.Cliente.Apellido}".Trim(),
                MontoServicio = x.MontoServicio,
                ComisionPlataforma = x.Comision,
                TotalPagadoCliente = x.Monto,
                GananciaProveedor = x.MontoServicio,
                Moneda = x.Moneda,
                FechaPago = x.FechaPago!.Value
            }).ToList();

        var response = new GananciasProveedorResponse
        {
            TotalServiciosPagados = pagos.Count,
            TotalGanadoProveedor = pagos.Sum(x => x.MontoServicio),
            TotalComisionesPlataforma = pagos.Sum(x => x.Comision),
            TotalCobradoClientes = pagos.Sum(x => x.Monto),
            Movimientos = movimientos
        };

        return Result<PagedResponse<GananciasProveedorResponse>>.Ok(
            PagedResponse<GananciasProveedorResponse>.Create(
                response, filtro.Page, filtro.PageSize, pagos.Count),
            "Ganancias del proveedor obtenidas correctamente.");
    }

    public async Task<Result<PagedResponse<HistorialPagosClienteResponse>>> ObtenerHistorialPagosClienteAsync(
        Guid clienteId, HistorialPagosClienteFiltroRequest filtro, CancellationToken ct = default)
    {
        if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde > filtro.Hasta)
            return Result<PagedResponse<HistorialPagosClienteResponse>>.Fail(
                "La fecha inicial no puede ser posterior a la fecha final.");

        var desde = filtro.Desde?.ToUniversalTime();
        var hasta = filtro.Hasta?.ToUniversalTime();
        var pagos = await _unitOfWork.Repository<Pago>().GetAsync(
            x => x.Activo && x.Estado == EstadoPago.Pagado && x.FechaPago.HasValue &&
                 x.SolicitudServicio.ClienteId == clienteId &&
                 (!desde.HasValue || x.FechaPago >= desde.Value) &&
                 (!hasta.HasValue || x.FechaPago <= hasta.Value),
            q => q.OrderByDescending(x => x.FechaPago), ct,
            x => x.SolicitudServicio.Servicio,
            x => x.SolicitudServicio.Servicio.Usuario);

        var movimientos = pagos
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .Select(x => new PagoClienteDto
            {
                PagoId = x.Id,
                SolicitudServicioId = x.SolicitudServicioId,
                Servicio = x.SolicitudServicio.Servicio.Titulo,
                Proveedor = $"{x.SolicitudServicio.Servicio.Usuario.Nombre} {x.SolicitudServicio.Servicio.Usuario.Apellido}".Trim(),
                MontoServicio = x.MontoServicio,
                ComisionPlataforma = x.Comision,
                TotalPagado = x.Monto,
                Moneda = "DOP",
                Estado = x.Estado,
                FechaPago = x.FechaPago!.Value
            }).ToList();

        var response = new HistorialPagosClienteResponse
        {
            TotalPagos = pagos.Count,
            TotalServicios = pagos.Sum(x => x.MontoServicio),
            TotalComisiones = pagos.Sum(x => x.Comision),
            TotalPagado = pagos.Sum(x => x.Monto),
            Moneda = "DOP",
            Pagos = movimientos
        };

        return Result<PagedResponse<HistorialPagosClienteResponse>>.Ok(
            PagedResponse<HistorialPagosClienteResponse>.Create(
                response, filtro.Page, filtro.PageSize, pagos.Count),
            "Historial de pagos del cliente obtenido correctamente.");
    }

    public async Task<Result> ProcesarWebhookAsync(string contenido, string firma, CancellationToken ct = default)
    {
        EventoPagoPasarela evento;
        try { evento = _pasarela.ConstruirEventoWebhook(contenido, firma); }
        catch (Exception ex) { return Result.Fail($"Webhook de Stripe inválido: {ex.Message}"); }

        if (evento.Tipo is not ("payment_intent.succeeded" or "payment_intent.payment_failed" or "payment_intent.processing"))
            return Result.Ok("Evento ignorado.");

        var pago = (await _unitOfWork.Repository<Pago>().GetTrackedAsync(
            x => x.ReferenciaTransaccion == evento.PaymentIntentId && x.Activo, null, ct,
            x => x.SolicitudServicio, x => x.SolicitudServicio.Servicio)).FirstOrDefault();
        if (pago is null) return Result.Fail("No existe un pago local para esta intención de Stripe.");
        return await AplicarEstadoPasarelaAsync(pago, evento.Estado, evento.UltimoError, ct);
    }

    private async Task<Result> AplicarEstadoPasarelaAsync(Pago pago, string estado, string? ultimoError, CancellationToken ct)
    {
        pago.EstadoPasarela = estado;
        pago.UltimoError = ultimoError?.Length > 500 ? ultimoError[..500] : ultimoError;
        pago.FechaModificacion = DateTime.UtcNow;
        if (estado == "succeeded")
        {
            if (pago.Estado == EstadoPago.Pagado && pago.SolicitudServicio.Estado == EstadoSolicitudServicio.Completada)
                return Result.Ok("El pago ya había sido procesado.");
            pago.Estado = EstadoPago.Pagado;
            pago.FechaPago = DateTime.UtcNow;
            pago.SolicitudServicio.Estado = EstadoSolicitudServicio.Completada;
            pago.SolicitudServicio.FechaModificacion = DateTime.UtcNow;
            var recompensa = await _recompensaService.AcreditarPuntosPorServicioCompletadoAsync(pago.SolicitudServicio, ct);
            if (!recompensa.Success) return recompensa;
            var progreso = await _progresoProveedorService.RegistrarServicioCompletadoAsync(pago.SolicitudServicio, ct);
            if (!progreso.Success) return progreso;
        }
        else if (estado == "processing") pago.Estado = EstadoPago.Retenido;
        else if (estado is "requires_payment_method" or "canceled") pago.Estado = EstadoPago.Fallido;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("Evento de pago procesado correctamente.");
    }

    private async Task<SolicitudServicio?> ObtenerSolicitudAsync(Guid id, CancellationToken ct) =>
        (await _unitOfWork.Repository<SolicitudServicio>().GetAsync(x => x.Id == id && x.Activo,
            null, ct, x => x.Servicio)).FirstOrDefault();

    private ResumenPagoDto CrearResumen(SolicitudServicio solicitud)
    {
        var montoServicio = decimal.Round(solicitud.PrecioAcordado!.Value, 2, MidpointRounding.AwayFromZero);
        var comision = decimal.Round(montoServicio * (_pasarela.PorcentajeComision / 100m), 2, MidpointRounding.AwayFromZero);
        return new ResumenPagoDto
        {
            SolicitudServicioId = solicitud.Id, MontoServicio = montoServicio,
            PorcentajeComision = _pasarela.PorcentajeComision, Comision = comision,
            TotalPagar = montoServicio + comision, Moneda = _pasarela.Moneda,
            EstadoSolicitud = solicitud.Estado, PuedePagar = solicitud.Estado == EstadoSolicitudServicio.PendientePago
        };
    }

    private static IntencionPagoDto MapIntencion(Pago pago, ResumenPagoDto resumen, IntencionPasarela intencion) => new()
    {
        PagoId = pago.Id, SolicitudServicioId = resumen.SolicitudServicioId,
        MontoServicio = resumen.MontoServicio, PorcentajeComision = resumen.PorcentajeComision,
        Comision = resumen.Comision, TotalPagar = resumen.TotalPagar, Moneda = resumen.Moneda,
        EstadoSolicitud = resumen.EstadoSolicitud, PuedePagar = resumen.PuedePagar,
        PaymentIntentId = intencion.Id, ClientSecret = intencion.ClientSecret, EstadoPasarela = intencion.Estado
    };

    private static PagoDetalleDto MapPago(Pago x) => new()
    {
        Id = x.Id, SolicitudServicioId = x.SolicitudServicioId, MontoServicio = x.MontoServicio,
        Comision = x.Comision, MontoTotal = x.Monto, Moneda = x.Moneda, Estado = x.Estado,
        EstadoPasarela = x.EstadoPasarela, UltimoError = x.UltimoError, FechaPago = x.FechaPago
    };
}
