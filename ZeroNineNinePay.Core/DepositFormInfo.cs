using Tgs.Payment.Clients;

namespace Tgs.Payment.ZeroNineNinePay
{
    public record DepositFormInfo(
        Guid Id,
       string PlayerCode,
       Guid BrandId,
       Guid BrandPaymentId,
       decimal Amount,
       PaymentFlowState Status,
       ZeroNineNinePayDepositApplyInfo? ApplyInfo,
       string? PaymentReturn = null);
}