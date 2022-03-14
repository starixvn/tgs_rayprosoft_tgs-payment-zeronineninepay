using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class ZeroNineNinePayDepositFailResponse : ZeroNineNinePayDepositResponse
    {
        [JsonPropertyName("data")]
        public new DepositResponseData[] Data { get; set; } = default!;
    }
}