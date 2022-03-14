namespace Tgs.Payment.ZeroNineNinePay.Services
{
    public record CheckSigningCodeCommand(
        IDictionary<string, object> Data,
        string SigningCode,
        string ApiKey)
    {
        public TaskCompletionSource<bool> CommandResultSource { get; } = new TaskCompletionSource<bool>();
    };
}