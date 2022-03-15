using System.Text.Json.Serialization;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class DepositOrder
    {
        /// <summary>
        /// Merchant ID
        /// </summary>
        /// <value>
        /// The merchant identifier.
        /// </value>
        [JsonPropertyName("merchant_id")]
        public int MerchantId { get; set; }

        /// <summary>
        /// Payment type:
        /// BANK_TRANSFER
        /// GCASH_TRANSFER
        /// GCASH_SCAN
        /// HELLO_TRANSFER
        /// </summary>
        /// <value>
        /// The type of the pay.
        /// </value>
        [JsonPropertyName("pay_type")]
        public string PayType { get; set; } = default!;

        /// <summary>
        /// Payer name(The real name of the payer, if it is empty or not sent, it needs to be entered manually on the payment page)
        /// </summary>
        /// <value>
        /// The name of the pay.
        /// </value>
        [JsonPropertyName("pay_name")]
        public string? PayName { get; set; }

        /// <summary>
        /// Merchant order number, within 32 digits
        /// </summary>
        /// <value>
        /// The out trade no.
        /// </value>
        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; set; } = default!;

        /// <summary>
        /// Asynchronous notification url, within 120 bits
        /// </summary>
        /// <value>
        /// The notify URL.
        /// </value>
        [JsonPropertyName("notify_url")]
        public string NotifyUrl { get; set; } = default!;

        /// <summary>
        /// Synchronous notification url, within 120 bits
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; } = default!;

        /// <summary>
        /// The submitted amount of deposit, unit:₱, Up to two decimal places
        /// </summary>
        /// <value>
        /// The money.
        /// </value>
        [JsonPropertyName("money")]
        public decimal Money { get; set; }

        /// <summary>
        /// User client IP of payer
        /// </summary>
        /// <value>
        /// The client ip.
        /// </value>
        [JsonPropertyName("client_ip")]
        public string ClientIp { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sign.
        /// </summary>
        /// <value>
        /// The sign.
        /// </value>
        [JsonPropertyName("sign")]
        public string Sign { get; set; } = default!;

        [JsonPropertyName("sign_type")]
        public string SignType { get; set; } = default!;
    }
}