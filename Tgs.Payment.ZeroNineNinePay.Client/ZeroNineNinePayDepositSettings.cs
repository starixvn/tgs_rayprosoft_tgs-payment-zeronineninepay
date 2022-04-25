namespace Tgs.Payment.ZeroNineNinePay
{
    public record ZeroNineNinePayDepositSettings(
        int MerchantId,
        string MerchantKey,
        string PayType,
        Guid OrderNo,
        string ClientIp,
        string? PayerName);
}