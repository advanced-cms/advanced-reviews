using System.Security.Cryptography;
using System.Text;

namespace Advanced.CMS.ExternalReviews.PinCodeSecurity
{
    internal static class PinCodeHashGenerator
    {
        public static string Hash(string pinCode, string token)
        {
            var value = token.Substring(0, 4) + pinCode + token.Substring(5, 4);

            var builder = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                foreach (var b in result)
                {
                    builder.Append(b.ToString("x2"));
                }
            }

            return builder.ToString();
        }
    }
}
