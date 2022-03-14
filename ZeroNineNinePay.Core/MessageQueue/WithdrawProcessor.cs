using System.Text.Json;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.ZeroNineNinePay.MessageQueue
{
    internal class WithdrawProcessor : IMessageProcessor
    {
        private readonly ZeroNineNinePayClient _zeroNineNinePayClient;
        private readonly ZeroNineNinePayOptions _options;

        public WithdrawProcessor(
            ZeroNineNinePayClient zeroNineNinePayClient,
            IOptions<ZeroNineNinePayOptions> optionsAccessor)
        {
            _zeroNineNinePayClient = zeroNineNinePayClient ?? throw new ArgumentNullException(nameof(zeroNineNinePayClient));
            _options = optionsAccessor.Value;
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var request = WithdrawRequest.Parser.ParseFrom(data.Span);
            var settings = JsonSerializer.Deserialize<ZeroNineNinePayWithdrawSettings>(request.ApplyInfo)!;

            var order = new WithdrawOrder
            {
                MerchantId = settings.MerchantId,
                OutTradeNo = settings.OrderNo.ToString(),
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

                //var transactionQueryOrder = new TransactionQueryOrder()
                //{
                //    Version = "V2",
                //    SignType = "MD5",
                //    MerchantNo = settings.MerchantNo,
                //    Date = DateTime.Now.ToString("yyyyMMddhhmmss"),
                //    OrderNo = settings.OrderNo.ToString()
                //};
                //var transactionQueryResponse = await _zeroNineNinePayClient
                //    .TransactionQueryAsync(settings.ApiKey, transactionQueryOrder)
                //    .ConfigureAwait(false);

                //var thirdPartyOrderNo = "";
                //if (transactionQueryResponse!.Code == "0")
                //{
                //    thirdPartyOrderNo = transactionQueryResponse!.Detail!.SysOrderNo;
                //}
                //else
                //{
                //    _logger.LogInformation($"transactionQueryResponse Failed:{transactionQueryResponse.Msg}");
                //}

                //await _paymentClient
                //    .SaveWithdrawFormThirdPartyOrderNoAsync(settings.OrderNo, thirdPartyOrderNo!)
                //    .ConfigureAwait(false);

                var msg = new WithdrawResponse
                {
                    Result = JsonSerializer.Serialize(applyInfo)
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
                    Result = JsonSerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}