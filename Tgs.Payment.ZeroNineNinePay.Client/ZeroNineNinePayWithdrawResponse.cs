using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class ZeroNineNinePayWithdrawResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("data")]
        public WithdrawResponseData Data { get; set; } = default!;
    }
}