using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.IntegrationTests
{
    /// <summary>
    /// Helper to simplify test code
    /// </summary>
    public static class ServiceLocatorExtensionMethods
    {
        /// <summary>
        /// Use with using-statement to set ServiceLocator in async context
        /// </summary>
        public static IServiceScope CreateServiceLocatorScope(this IServiceCollection newServices) =>
            newServices.BuildServiceProvider().CreateServiceLocatorScope();
    }
}
