using System.Text.Json;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class GetWithdrawFormQueryService : IGetService<GetWithdrawFormQuery, WithdrawFormInfo?>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public GetWithdrawFormQueryService(
            IPaymentServiceClient paymentServiceClient,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer ?? throw new ArgumentNullException(nameof(zeroNineNinePaySerializer));
        }

        public async ValueTask<WithdrawFormInfo?> GetAsync(GetWithdrawFormQuery query, CancellationToken cancellationToken = default)
        {
            var withdrawFormId = Guid.Parse(query.OrderNo);

            var formData = await _paymentServiceClient
                .GetWithdrawFormAsync(withdrawFormId)
                .ConfigureAwait(false);

            return formData == null
                ? null
                : new WithdrawFormInfo(
                    formData.Id,
                    formData.UserId,
                    formData.UserType,
                    formData.BrandId,
                    formData.BrandPaymentId,
                    formData.Amount,
                    formData.Status,
                    formData.BankInfoId,
                    formData.BankInfo,
                    string.IsNullOrEmpty(formData.ApplyInfo)
                        ? null
                        : _zeroNineNinePaySerializer.Deserialize<ZeroNineNinePayWithdrawApplyInfo>(formData.ApplyInfo),
                    formData.PaymentReturn);

        }
    }
}