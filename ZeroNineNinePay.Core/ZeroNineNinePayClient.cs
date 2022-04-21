using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.ZeroNineNinePay
{
    internal class ZeroNineNinePayClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ZeroNineNinePayClient> _logger;
        private readonly ZeroNineNinePaySerializer _zeroNineNinePaySerializer;

        public ZeroNineNinePayClient(
            HttpClient httpClient, 
            ILogger<ZeroNineNinePayClient> logger,
            ZeroNineNinePaySerializer zeroNineNinePaySerializer)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _zeroNineNinePaySerializer = zeroNineNinePaySerializer ?? throw new ArgumentNullException(nameof(zeroNineNinePaySerializer));
        }

        public async ValueTask<ZeroNineNinePayDepositResponse?> DepositAsync(string merchantKey, DepositOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order);

            _logger.LogInformation($"deposit orderJson:{orderJson}");

            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson)!;

            var sign = ZeroNineNinePayHasher.ComputeHash(merchantKey, orderDict);

            orderDict["sign"] = sign;

            var content = new StringContent(JsonSerializer.Serialize(orderDict));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient
                .PostAsync("/gateway/pay/index.html", content)
                .ConfigureAwait(false);

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            _logger.LogInformation($"deposit bodyJson:{bodyJson}");

            var apiResponseData = _zeroNineNinePaySerializer.Deserialize<ZeroNineNinePayDepositResponse>(bodyJson);

            return apiResponseData;
        }

        public async ValueTask<ZeroNineNinePayWithdrawResponse?> WithdrawAsync(string merchantKey, WithdrawOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order);

            _logger.LogInformation("withdraw orderJson:{orderJson}", orderJson);

            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson)!;

            var sign = ZeroNineNinePayHasher.ComputeHash(merchantKey, orderDict);

            _logger.LogInformation($"withdraw sign:{sign}");

            orderDict["sign"] = sign;

            var content = new StringContent(JsonSerializer.Serialize(orderDict));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient
                .PostAsync("/gateway/withdraw/index.html", content)
                .ConfigureAwait(false);

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            _logger.LogInformation($"withdraw bodyJson:{bodyJson}");

            var apiResponseData = _zeroNineNinePaySerializer.Deserialize<ZeroNineNinePayWithdrawResponse>(bodyJson);

            return apiResponseData;
        }
    }
}