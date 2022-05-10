namespace Tgs.Payment.ZeroNineNinePay;

public class ZeroNineNinePayOptions
{
    public string CallbackBaseUrl { get; set; } = default!;
    public string NatsUrl { get; set; } = default!;
    public Uri PaymentApiUrl { get; set; } = default!;
    public Uri PaymentServiceBaseUrl { get; set; } = default!;
}
