using ConectaMasPro.Domain.Enums;
using ConectaMasPro.Application.Common.Pagination;

namespace ConectaMasPro.Application.DTOs.Pagos;

public class ResumenPagoDto
{
    public Guid SolicitudServicioId { get; set; }
    public decimal MontoServicio { get; set; }
    public decimal PorcentajeComision { get; set; }
    public decimal Comision { get; set; }
    public decimal TotalPagar { get; set; }
    public string Moneda { get; set; } = null!;
    public EstadoSolicitudServicio EstadoSolicitud { get; set; }
    public bool PuedePagar { get; set; }
}

public class IntencionPagoDto : ResumenPagoDto
{
    public Guid PagoId { get; set; }
    public string PaymentIntentId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string EstadoPasarela { get; set; } = null!;
}

public class ConfiguracionPublicaStripeDto
{
    public string ClavePublicable { get; set; } = null!;
    public string Moneda { get; set; } = null!;
    public decimal PorcentajeComision { get; set; }
}

public class PagoDetalleDto
{
    public Guid Id { get; set; }
    public Guid SolicitudServicioId { get; set; }
    public decimal MontoServicio { get; set; }
    public decimal Comision { get; set; }
    public decimal MontoTotal { get; set; }
    public string Moneda { get; set; } = null!;
    public EstadoPago Estado { get; set; }
    public string? EstadoPasarela { get; set; }
    public string? UltimoError { get; set; }
    public DateTime? FechaPago { get; set; }
}

public class GananciasProveedorFiltroRequest : PagedRequest
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GananciaProveedorDto
{
    public Guid PagoId { get; set; }
    public Guid SolicitudServicioId { get; set; }
    public string Servicio { get; set; } = null!;
    public string Cliente { get; set; } = null!;
    public decimal MontoServicio { get; set; }
    public decimal ComisionPlataforma { get; set; }
    public decimal TotalPagadoCliente { get; set; }
    public decimal GananciaProveedor { get; set; }
    public string Moneda { get; set; } = null!;
    public DateTime FechaPago { get; set; }
}

public class GananciasProveedorResponse
{
    public int TotalServiciosPagados { get; set; }
    public decimal TotalGanadoProveedor { get; set; }
    public decimal TotalComisionesPlataforma { get; set; }
    public decimal TotalCobradoClientes { get; set; }
    public IReadOnlyList<GananciaProveedorDto> Movimientos { get; set; } = [];
}

public class HistorialPagosClienteFiltroRequest : PagedRequest
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class PagoClienteDto
{
    public Guid PagoId { get; set; }
    public Guid SolicitudServicioId { get; set; }
    public string Servicio { get; set; } = null!;
    public string Proveedor { get; set; } = null!;
    public decimal MontoServicio { get; set; }
    public decimal ComisionPlataforma { get; set; }
    public decimal TotalPagado { get; set; }
    public string Moneda { get; set; } = "DOP";
    public EstadoPago Estado { get; set; }
    public DateTime FechaPago { get; set; }
}

public class HistorialPagosClienteResponse
{
    public int TotalPagos { get; set; }
    public decimal TotalServicios { get; set; }
    public decimal TotalComisiones { get; set; }
    public decimal TotalPagado { get; set; }
    public string Moneda { get; set; } = "DOP";
    public IReadOnlyList<PagoClienteDto> Pagos { get; set; } = [];
}
