using ConectaMasPro.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;

namespace ConectaMasPro.Infrastructure.Payments;

public class StripePaymentService : IPasarelaPagoService
{
    private readonly StripeSettings _settings;

    public StripePaymentService(IOptions<StripeSettings> options)
    {
        _settings = options.Value;
    }

    public string ClavePublicable => _settings.PublishableKey;
    public string Moneda => _settings.Currency.Trim().ToLowerInvariant();
    public decimal PorcentajeComision => _settings.CommissionPercentage;

    public async Task<IntencionPasarela> CrearIntencionAsync(
        long montoMenorUnidad,
        string descripcion,
        IReadOnlyDictionary<string, string> metadata,
        string claveIdempotencia,
        CancellationToken ct = default)
    {
        if (montoMenorUnidad <= 0) throw new InvalidOperationException("El monto para Stripe debe ser mayor que cero.");
        var intent = await CrearServicio().CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = montoMenorUnidad,
            Currency = Moneda,
            Description = descripcion,
            PaymentMethodTypes = ["card"],
            Metadata = metadata.ToDictionary(x => x.Key, x => x.Value)
        }, new RequestOptions { IdempotencyKey = claveIdempotencia }, ct);
        return Map(intent);
    }

    public async Task<IntencionPasarela> ObtenerIntencionAsync(string paymentIntentId, CancellationToken ct = default)
    {
        var intent = await CrearServicio().GetAsync(paymentIntentId, null, null, ct);
        return Map(intent);
    }

    public EventoPagoPasarela ConstruirEventoWebhook(string contenido, string firma)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret))
            throw new InvalidOperationException("Stripe:WebhookSecret no está configurado.");
        var stripeEvent = EventUtility.ConstructEvent(contenido, firma, _settings.WebhookSecret);
        if (stripeEvent.Data.Object is not PaymentIntent intent)
            return new EventoPagoPasarela(stripeEvent.Type, string.Empty, string.Empty, null);
        return new EventoPagoPasarela(stripeEvent.Type, intent.Id, intent.Status, intent.LastPaymentError?.Message);
    }

    private string ObtenerSecretKey()
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey) || !_settings.SecretKey.StartsWith("sk_test_", StringComparison.Ordinal))
            throw new InvalidOperationException("Configura una clave Stripe de prueba válida en Stripe:SecretKey.");
        return _settings.SecretKey;
    }

    private PaymentIntentService CrearServicio() =>
        new(new StripeClient(ObtenerSecretKey()));

    private static IntencionPasarela Map(PaymentIntent intent) =>
        new(intent.Id, intent.ClientSecret, intent.Status, intent.LastPaymentError?.Message);
}
