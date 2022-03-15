using NUnit.Framework;

namespace Tgs.Payment.ZeroNineNinePay.Tests
{
    public class HasherTests
    {
        [Test]
        public void Test_Check_Deposit_Hash()
        {
            var clientIp = "220.132.206.70";
            var merchantId = 20086;
            var money = 300.35m;
            var notifyUrl = "https://www.baidu.com";
            var outTradeNo = "TEST22031114194830";
            var payName = "Musk";
            var payType = "BANK_TRANSFER";
            var returnUrl = "https://www.baidu.com";

            var dict = new Dictionary<string, object>
            {
                { "client_ip",clientIp },
                { "merchant_id",merchantId },
                { "money",money},
                { "notify_url", notifyUrl},
                { "out_trade_no", outTradeNo},
                { "pay_name", payName},
                { "pay_type", payType},
                { "return_url", returnUrl},
                { "sign", "e088f6ac243b21c3a23f982558386cd1"},
                { "sign_type", "md5"}
            };

            var merchantKey = "2c7541a758a70cdf47a31f405120f5e0";
            var actual = ZeroNineNinePayHasher.ComputeHash(merchantKey, dict);

            var expected = "e088f6ac243b21c3a23f982558386cd1";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Check_Withdraw_Hash()
        {
            var merchantId = 20086;
            var bankAddress = "none";
            var bankName = "aub";
            var bankNumber = "6217005345642456000";
            var money = 5200m;
            var notifyUrl = "https://www.baidu.com";
            var outTradeNo = "TEST22031413351190";
            var realname = "Musk";

            var dict = new Dictionary<string, object>
            {
                { "merchant_id",merchantId },
                { "bank_address",bankAddress },
                { "bank_name",bankName },
                { "bank_number",bankNumber },
                { "money",money},
                { "notify_url", notifyUrl},
                { "out_trade_no", outTradeNo},
                { "realname", realname},
                { "sign_type", "md5"}
            };

            var merchantKey = "2c7541a758a70cdf47a31f405120f5e0";
            var actual = ZeroNineNinePayHasher.ComputeHash(merchantKey, dict);

            var expected = "72449dfa79daec62a1b39ca1a60f13d3";
            Assert.AreEqual(expected, actual);
        }
    }
}