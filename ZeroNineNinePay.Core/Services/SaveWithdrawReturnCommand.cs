using Tgs.Payment.Clients;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record SaveWithdrawReturnCommand(
        Guid FormId,
        PaymentFlowState State,
        ZeroNineNinePayWithdrawReturn WithdrawReturn,
        decimal? ActualAmount);
}