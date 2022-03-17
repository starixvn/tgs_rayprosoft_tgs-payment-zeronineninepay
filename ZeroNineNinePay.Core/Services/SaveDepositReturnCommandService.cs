using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class SaveDepositReturnCommandService : ICommandService<SaveDepositReturnCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public SaveDepositReturnCommandService(
            IPaymentServiceClient paymentServiceClient,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer ?? throw new ArgumentNullException(nameof(zeroNineNinePaySerializer));
        }

        public async ValueTask ExecuteAsync(SaveDepositReturnCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentServiceClient
                .GetDepositFormAsync(command.FormId)
                .ConfigureAwait(false);

            if (formData == null)
                return;

            await _paymentServiceClient
                .SubmitDepositFormStatusAsync(
                    command.FormId,
                    formData.Status,
                    _zeroNineNinePaySerializer.Serialize(command.DepositReturn))
                .ConfigureAwait(false);
        }
    }
}