namespace Tgs.Payment.ZeroNineNinePay
{
    public record ZeroNineNinePayWithdrawSettings(
        int MerchantId,
        string MerchantKey,
        string RealName,
        string BankName,
        string BankNumber,
        string? BankAddress,
        Guid OrderNo);
}