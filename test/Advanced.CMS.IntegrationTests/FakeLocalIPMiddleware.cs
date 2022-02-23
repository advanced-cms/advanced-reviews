using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.IntegrationTests
{
    public class FakeLocalIPMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IPAddress fakeIpAddress = IPAddress.Parse("127.168.1.32");

        public FakeLocalIPMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Connection.LocalIpAddress = fakeIpAddress;

            await next(httpContext);
        }
    }
}
