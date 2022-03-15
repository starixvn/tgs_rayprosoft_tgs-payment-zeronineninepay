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
            //await DepoistAsync();
            await WithdrawAsync();
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

        private async Task WithdrawAsync()
        {
            var merchantId = 20114;
            var merchantKey = "e966245afe6abbb1b531774b088e7d58";
            var realName = "realNameTest";
            var bankName = "bankNameTest";
            var bankNumber = "112233445566";
            var bankAddress = "bankAddressTest";            
            var orderNo = Guid.NewGuid();
            var amount = 1000m;

            var withdrawSettings = new ZeroNineNinePayWithdrawSettings(
                merchantId,
                merchantKey,
                realName,
                bankName,
                bankNumber,
                bankAddress,
                orderNo);
            
            try
            {
                var withdrawReturn = await _paymentClient.WithdrawAsync(
                    "tgs.payment.zeronineninepay.withdraw",
                    amount,
                    JsonSerializer.Serialize(withdrawSettings));
                _logger.LogInformation($"Withdraw Return: {withdrawReturn}");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
            }
        }
    }
}