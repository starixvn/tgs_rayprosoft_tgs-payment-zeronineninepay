using System.Net.Http.Headers;
using System.Text.Json;

namespace Tgs.Payment.ZeroNineNinePay
{
    internal class ZeroNineNinePayClient
    {
        private readonly HttpClient _httpClient;

        public ZeroNineNinePayClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask<ZeroNineNinePayDepositResponse?> DepositAsync(string merchantKey, DepositOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order);
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

            var apiResponseData = JsonSerializer.Deserialize<ZeroNineNinePayDepositResponse>(bodyJson);

            return apiResponseData;
        }

        public async ValueTask<ZeroNineNinePayWithdrawResponse?> WithdrawAsync(string merchantKey, WithdrawOrder order)
        {
            var orderJson = JsonSerializer.Serialize(order);
            var orderDict = JsonSerializer
                .Deserialize<Dictionary<string, object>>(orderJson)!;

            var sign = ZeroNineNinePayHasher.ComputeHash(merchantKey, orderDict);

            orderDict["sign"] = sign;

            var content = new StringContent(JsonSerializer.Serialize(orderDict));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient
                .PostAsync("/gateway/withdraw/index.html", content)
                .ConfigureAwait(false);

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _ = response.EnsureSuccessStatusCode();

            var apiResponseData = JsonSerializer.Deserialize<ZeroNineNinePayWithdrawResponse>(bodyJson);

            return apiResponseData;
        }
    }
}