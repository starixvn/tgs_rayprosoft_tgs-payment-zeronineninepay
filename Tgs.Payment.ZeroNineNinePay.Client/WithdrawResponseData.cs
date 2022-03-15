using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class WithdrawResponseData
    {
        [JsonPropertyName("merchant_id")]
        public int MerchantId { get; set; }

        [JsonPropertyName("trade_no")]
        public string TradeNo { get; set; } = default!;

        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; set; } = default!;

        [JsonPropertyName("money")]
        public decimal Money { get; set; }

        [JsonPropertyName("fee")]
        public string Fee { get; set; } = default!;

        [JsonPropertyName("sign_type")]
        public string SignType { get; set; } = default!;

        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;
    }
}