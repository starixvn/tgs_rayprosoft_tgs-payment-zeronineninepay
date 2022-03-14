namespace Tgs.Payment.ZeroNineNinePay
{
    public class ZeroNineNinePayWithdrawReturn
    {
        public int MerchantId { get; init; }

        public string TradeNo { get; init; } = default!;

        public string OutTradeNo { get; init; } = default!;

        public decimal Money { get; init; }

        public decimal Fee { get; init; }

        public int State { get; init; }

        public string Sign { get; init; } = default!;

        public string SignType { get; init; } = default!;
    }
}