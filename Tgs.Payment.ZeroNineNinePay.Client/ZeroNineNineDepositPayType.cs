using System.ComponentModel;

namespace Tgs.Payment.ZeroNineNinePay
{
    public enum ZeroNineNineDepositPayType
    {
        [Description("BANK_TRANSFER")]
        BANK_TRANSFER,

        [Description("GCASH_TRANSFER")]
        GCASH_TRANSFER,

        [Description("GCASH_SCAN")]
        GCASH_SCAN
    }
}