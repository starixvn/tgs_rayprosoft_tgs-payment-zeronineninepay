using Tgs.Payment.Clients;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record SaveWithdrawReturnCommand(
        Guid FormId,        
        ZeroNineNinePayWithdrawReturn WithdrawReturn,
        decimal? ActualAmount);
}