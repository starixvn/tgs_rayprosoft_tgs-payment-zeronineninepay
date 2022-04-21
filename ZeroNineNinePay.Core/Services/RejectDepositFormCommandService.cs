using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class RejectDepositFormCommandService : ICommandService<RejectDepositFormCommand>
    {
        private readonly IPaymentClient _paymentClient;

        public RejectDepositFormCommandService(IPaymentClient paymentClient)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        }

        public ValueTask ExecuteAsync(RejectDepositFormCommand command, CancellationToken cancellationToken = default)
            => _paymentClient.RejectDepositAsync(
                command.FormId,
                command.BrandId);
    }
}