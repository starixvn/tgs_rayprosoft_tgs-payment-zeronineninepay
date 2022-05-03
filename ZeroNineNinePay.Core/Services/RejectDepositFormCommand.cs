namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record RejectDepositFormCommand(Guid FormId, Guid BrandId);
}