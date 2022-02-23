using System;
using System.Net.Http;

namespace Advanced.CMS.IntegrationTests
{
    public static class HttpClientExtension
    {
        public static HttpResponseMessage GetSync(this HttpClient client, string url) => client.GetAsync(url).GetAwaiter().GetResult();

        public static HttpResponseMessage GetSync(this HttpClient client, Uri url) => client.GetAsync(url).GetAwaiter().GetResult();
    }
}
