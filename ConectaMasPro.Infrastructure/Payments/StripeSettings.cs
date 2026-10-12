namespace ConectaMasPro.Infrastructure.Payments;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string Currency { get; set; } = "dop";
    public decimal CommissionPercentage { get; set; } = 10m;
}
