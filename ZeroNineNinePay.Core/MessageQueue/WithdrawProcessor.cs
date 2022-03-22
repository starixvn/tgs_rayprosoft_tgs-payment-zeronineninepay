using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace Tgs.Payment.ZeroNineNinePay.MessageQueue
{
    internal class WithdrawProcessor : IMessageProcessor
    {
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;
        private readonly ZeroNineNinePayClient _zeroNineNinePayClient;
        private readonly IPaymentClient _paymentClient;
        private readonly ZeroNineNinePayOptions _options;

        public WithdrawProcessor(
            ZeroNineNinePaySerializer zeroNineNinePaySerializer,
            ZeroNineNinePayClient zeroNineNinePayClient,
            IOptions<ZeroNineNinePayOptions> optionsAccessor,
            IPaymentClient paymentClient)
        {
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer;
            _zeroNineNinePayClient = zeroNineNinePayClient ?? throw new ArgumentNullException(nameof(zeroNineNinePayClient));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _options = optionsAccessor.Value;
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var request = WithdrawRequest.Parser.ParseFrom(data.Span);
            var settings = _zeroNineNinePaySerializer.Deserialize<ZeroNineNinePayWithdrawSettings>(request.ApplyInfo)!;

            var order = new WithdrawOrder
            {
                MerchantId = settings.MerchantId,
                OutTradeNo = settings.OrderNo.ToString("N"),
                Realname = settings.RealName,
                BankName = settings.BankName,
                BankNumber = settings.BankNumber,
                BankAddress = settings.BankAddress,
                NotifyUrl = $"{_options.CallbackBaseUrl}/withdraw/callback",
                Money = decimal.Parse(request.Amount),
                SignType = "md5"
            };

            var withdrawResponse = await _zeroNineNinePayClient
                .WithdrawAsync(settings.MerchantKey, order)
                .ConfigureAwait(false);

            if (withdrawResponse != null && withdrawResponse.Code == 200)
            {
                var applyInfo = new ZeroNineNinePayWithdrawApplyInfo(settings, withdrawResponse);

                await _paymentClient
                    .SaveWithdrawFormThirdPartyOrderNoAsync(settings.OrderNo, withdrawResponse.Data.TradeNo)
                    .ConfigureAwait(false);

                var msg = new WithdrawResponse
                {
                    Result = _zeroNineNinePaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
            else
            {
                var response = new ZeroNineNinePayWithdrawFailResponse
                {
                    Code = withdrawResponse!.Code,
                    Message = withdrawResponse!.Message
                };

                var applyInfo = new ZeroNineNinePayWithdrawApplyInfo(settings, response);

                var msg = new WithdrawResponse
                {
                    Result = _zeroNineNinePaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}