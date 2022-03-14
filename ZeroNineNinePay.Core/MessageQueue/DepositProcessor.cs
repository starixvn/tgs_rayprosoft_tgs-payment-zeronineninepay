using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.ZeroNineNinePay.MessageQueue
{
    internal class DepositProcessor : IMessageProcessor
    {
        private readonly ZeroNineNinePayClient _zeroNineNinePayClient;
        private readonly ZeroNineNinePayOptions _options;

        public DepositProcessor(
            ZeroNineNinePayClient zeroNineNinePayClient,
            IOptions<ZeroNineNinePayOptions> optionsAccessor)
        {
            _zeroNineNinePayClient = zeroNineNinePayClient ?? throw new ArgumentNullException(nameof(zeroNineNinePayClient));
            _options = optionsAccessor.Value;
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var request = DepositRequest.Parser.ParseFrom(data.Span);
            var settings = JsonSerializer.Deserialize<ZeroNineNinePayDepositSettings>(request.ApplyInfo)!;

            var order = new DepositOrder
            {
                MerchantId = settings.MerchantId,
                PayType = settings.PayType.ToString(),
                PayName = settings.PayerName,
                OutTradeNo = settings.OrderNo.ToString(),
                NotifyUrl = $"{_options.CallbackBaseUrl}/deposit/callback",
                ReturnUrl = null,
                Money = decimal.Parse(request.Amount),
                ClientIp = settings.ClientIp,
                SignType = "md5"
            };

            var depositResponse = await _zeroNineNinePayClient
                .DepositAsync(settings.MerchantKey, order)
                .ConfigureAwait(false);

            if (depositResponse != null && depositResponse.Code == 200)
            {
                var applyInfo = new ZeroNineNinePayDepositApplyInfo(settings, depositResponse);

                var msg = new DepositResponse
                {
                    Result = JsonSerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
            else
            {
                var errorMsg = string.Empty;
                if (depositResponse != null)
                    errorMsg = depositResponse.Message ?? "Request Fail";

                var response = new ZeroNineNinePayDepositFailResponse
                {
                    Code = depositResponse!.Code,
                    Message = errorMsg
                };

                var applyInfo = new ZeroNineNinePayDepositApplyInfo(settings, response);

                var msg = new DepositResponse
                {
                    Result = JsonSerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}