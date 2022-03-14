using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class ZeroNineNinePayDepositResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("data")]
        public DepositResponseData Data { get; set; } = default!;
    }
}