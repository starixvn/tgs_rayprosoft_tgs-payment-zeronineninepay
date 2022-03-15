using System.Security.Cryptography;
using System.Text;

namespace Tgs.Payment.ZeroNineNinePay
{
    public class ZeroNineNinePayHasher
    {
        private static readonly MD5 _Md5;

        static ZeroNineNinePayHasher()
        {
            _Md5 = MD5.Create();
        }

        public static string ComputeHash(string merchantKey, IDictionary<string, object> dict)
        {
            var content = string.Join(
                "&",
                dict.Where(x => (x.Key is not "sign" and not "sign_type") && x.Value is not null)
                    .OrderBy(x => x.Key).Select(p =>
                    {
                        return $"{p.Key}={p.Value}";
                    }));

            var plainText = $"{content}{merchantKey}";

            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _Md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }
    }
}