using System.Security.Cryptography;
using System.Text;

namespace ReactReduxApi.Helpers
{
    public static class CryptoHelper
    {
        public static string GetSha256String(string password)
        {
            return Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }
    }
}
