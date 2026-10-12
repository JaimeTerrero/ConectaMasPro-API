namespace ConectaMasPro.Application.DTOs.Solicitudes;

public class EstablecerPrecioAcordadoRequest
{
    public decimal PrecioAcordado { get; set; }
}

public class ConfirmarFinalizacionRequest
{
    public bool Confirmado { get; set; }
    public string? MotivoRechazo { get; set; }
}
