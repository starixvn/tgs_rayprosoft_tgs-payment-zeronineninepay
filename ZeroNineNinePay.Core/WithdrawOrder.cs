using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class WithdrawOrder
    {
        [JsonPropertyName("merchant_id")]
        public int MerchantId { get; set; }

        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; set; } = default!;

        [JsonPropertyName("realname")]
        public string Realname { get; set; } = default!;

        [JsonPropertyName("bank_name")]
        public string BankName { get; set; } = default!;

        [JsonPropertyName("bank_number")]
        public string BankNumber { get; set; } = default!;

        [JsonPropertyName("bank_address")]
        public string? BankAddress { get; set; }

        [JsonPropertyName("notify_url")]
        public string? NotifyUrl { get; set; }

        [JsonPropertyName("money")]
        public decimal Money { get; set; }

        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;

        [JsonPropertyName("sign_type")]
        public string SignType { get; set; } = default!;
    }
}