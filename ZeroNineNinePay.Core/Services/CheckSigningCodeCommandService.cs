using Tgs.Payment.ZeroNineNinePay;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Services
{
    internal class CheckSigningCodeCommandService : ICommandService<CheckSigningCodeCommand>
    {
        public CheckSigningCodeCommandService()
        {
        }

        public ValueTask ExecuteAsync(CheckSigningCodeCommand command, CancellationToken cancellationToken = default)
        {
            var signCode = ZeroNineNinePayHasher.ComputeHash(command.ApiKey, command.Data);

            command.CommandResultSource.SetResult(signCode == command.SigningCode);

            return ValueTask.CompletedTask;
        }
    }
}