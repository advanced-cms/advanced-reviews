using System.Net.Http;

namespace Advanced.CMS.IntegrationTests
{
    public static class HttpContentExtension
    {
        public static string ReadAsStringSync(this HttpContent content) => content.ReadAsStringAsync().GetAwaiter().GetResult();
    }
}
