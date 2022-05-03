using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class SaveWithdrawReturnCommandService : ICommandService<SaveWithdrawReturnCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public SaveWithdrawReturnCommandService(
            IPaymentServiceClient paymentServiceClient,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
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
                    formData.Id,
                    command.State,
                    _zeroNineNinePaySerializer.Serialize(command.WithdrawReturn),
                    command.ActualAmount)
                .ConfigureAwait(false);
        }
    }
}