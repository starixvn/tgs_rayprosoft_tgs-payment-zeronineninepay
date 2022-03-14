namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record ApproveDepositFormCommand(Guid FormId, Guid BrandId, decimal? ActualAmount);
}