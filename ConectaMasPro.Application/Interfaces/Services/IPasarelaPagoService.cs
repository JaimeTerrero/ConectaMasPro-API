namespace ConectaMasPro.Application.Interfaces.Services;

public record IntencionPasarela(
    string Id,
    string ClientSecret,
    string Estado,
    string? UltimoError);

public record EventoPagoPasarela(
    string Tipo,
    string PaymentIntentId,
    string Estado,
    string? UltimoError);

public interface IPasarelaPagoService
{
    string ClavePublicable { get; }
    string Moneda { get; }
    decimal PorcentajeComision { get; }
    Task<IntencionPasarela> CrearIntencionAsync(long montoMenorUnidad, string descripcion, IReadOnlyDictionary<string, string> metadata, string claveIdempotencia, CancellationToken ct = default);
    Task<IntencionPasarela> ObtenerIntencionAsync(string paymentIntentId, CancellationToken ct = default);
    EventoPagoPasarela ConstruirEventoWebhook(string contenido, string firma);
}
