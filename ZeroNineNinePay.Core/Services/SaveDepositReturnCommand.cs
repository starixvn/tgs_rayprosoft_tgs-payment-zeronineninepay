namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record SaveDepositReturnCommand(Guid FormId, ZeroNineNinePayDepositReturn DepositReturn);
}