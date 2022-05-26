using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class SaveWithdrawReturnCommandService : ICommandService<SaveWithdrawReturnCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPaymentClient _paymentClient;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public SaveWithdrawReturnCommandService(
            IPaymentServiceClient paymentServiceClient,
            IPaymentClient paymentClient,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer ?? throw new ArgumentNullException(nameof(zeroNineNinePaySerializer));
        }

        public async ValueTask ExecuteAsync(SaveWithdrawReturnCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentServiceClient
                .GetWithdrawFormAsync(command.FormId)
                .ConfigureAwait(false);

            if (formData == null)
                return;

            await _paymentServiceClient
                .SubmitWithdrawFormStatusAsync(
                    formId: formData.Id,
                    state: null,
                    paymentReturn: _zeroNineNinePaySerializer.Serialize(command.WithdrawReturn),
                    actualAmount: command.ActualAmount)
                .ConfigureAwait(false);

            await _paymentClient
                .PaidWithdrawAsync(
                    formData.Id,
                    formData.BrandId,
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}