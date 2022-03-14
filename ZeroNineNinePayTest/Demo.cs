using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.Payment.ZeroNineNinePay;

namespace ZeroNineNinePayTest
{
    internal class Demo : BackgroundService
    {
        private readonly IPaymentClient _paymentClient;
        private readonly ILogger<Demo> _logger;

        public Demo(
            IPaymentClient paymentClient,
            ILogger<Demo> logger)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DepoistAsync();
            //await WithdrawAsync();
        }

        private async Task DepoistAsync()
        {           
            var merchantId = 20114;
            var merchantKey = "e966245afe6abbb1b531774b088e7d58";
            var payType = ZeroNineNineDepositPayType.BANK_TRANSFER;
            var orderNo = Guid.NewGuid();
            var amount = 600.00m;
            var clientIp = "220.132.206.70";
            

            var depositSettings = new ZeroNineNinePayDepositSettings(
                merchantId,
                merchantKey,
                payType,
                orderNo,
                clientIp,
                PayerName: null);
            try
            {
                var depositReturn = await _paymentClient.DepositAsync(
                    "tgs.payment.zeronineninepay.deposit",
                    amount,
                    JsonSerializer.Serialize(depositSettings));
                _logger.LogInformation($"Deposit Return: {depositReturn}");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
            }
        }

        //private async Task WithdrawAsync()
        //{
        //    var merchantNo = "API7313181217337047";
        //    var apiKey = "68199f7df5e142c69ae2516b75c176d5";
        //    var orderNo = Guid.NewGuid();
        //    var accountName = "accountName";
        //    var bankCode = "ABC";
        //    var bankBranchName = "农业银行";
        //    var cardNo = "112233445566";
        //    var amount = 500;

        //    var withdrawSettings = new MaticPayWithdrawSettings(
        //        merchantNo,
        //        apiKey,
        //        orderNo,
        //        amount,
        //        accountName,
        //        bankCode,
        //        bankBranchName,
        //        cardNo);
        //    try
        //    {
        //        var withdrawReturn = await _paymentClient.WithdrawAsync(
        //            "tgs.payment.zeronineninepay.withdraw",
        //            amount,
        //            _maticPaySerializer.Serialize(withdrawSettings));
        //        _logger.LogInformation($"Withdraw Return: {withdrawReturn}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogDebug(ex.Message);
        //    }
        //}
    }
}