using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormInfo?>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public GetDepositFormQueryService(
            IPaymentServiceClient paymentServiceClient,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer ?? throw new ArgumentNullException(nameof(zeroNineNinePaySerializer));
        }

        public async ValueTask<DepositFormInfo?> GetAsync(GetDepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var formId = Guid.Parse(query.OrderNo);

            var formData = await _paymentServiceClient
                .GetDepositFormAsync(formId)
                .ConfigureAwait(false);

            return formData == null
                ? null
                : new DepositFormInfo(
                    formData.Id,
                    formData.PlayerCode,
                    formData.BrandId,
                    formData.BrandPaymentId,
                    formData.Amount,
                    formData.Status,
                    string.IsNullOrEmpty(formData.ApplyInfo)
                        ? null
                        : _zeroNineNinePaySerializer.Deserialize<ZeroNineNinePayDepositApplyInfo>(formData.ApplyInfo));
        }
    }
}