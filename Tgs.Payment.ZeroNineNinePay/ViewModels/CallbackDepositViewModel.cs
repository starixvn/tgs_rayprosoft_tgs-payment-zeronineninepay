using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay.ViewModels
{
    public class CallbackDepositViewModel
    {
        [JsonPropertyName("merchant_id")]
        public int MerchantId { get; init; }

        [JsonPropertyName("pay_type")]
        public string PayType { get; init; } = default!;

        [JsonPropertyName("trade_no")]
        public string TradeNo { get; init; } = default!;

        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; init; } = default!;

        [JsonPropertyName("money")]
        public decimal Money { get; init; }

        /// <summary>
        /// Actual payment amount, unit:₱, Up to two decimal places, Business processing must be subject to the actual amount
        /// </summary>
        [JsonPropertyName("money_true")]
        public decimal MoneyTrue { get; init; }

        [JsonPropertyName("fee")]
        public decimal Fee { get; init; }

        /// <summary>
        /// 1: succeed, 0: pending, -1: failed
        /// </summary>
        [JsonPropertyName("state")]
        public int State { get; init; }

        [JsonPropertyName("sign")]
        public string Sign { get; init; } = default!;

        [JsonPropertyName("sign_type")]
        public string SignType { get; init; } = default!;
    }
}