using Microsoft.AspNetCore.Mvc.Testing;

namespace Advanced.CMS.IntegrationTests
{
    public class ServiceFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
    }
}
