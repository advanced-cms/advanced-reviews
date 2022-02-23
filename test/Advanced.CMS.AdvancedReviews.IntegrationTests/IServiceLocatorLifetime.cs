using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests
{
    /// <summary>
    /// Helper interface class for legacy 'ServiceLocator.Current' tests. This exists for compatibility reasons, tests should not reference ServiceLocator.Current.
    /// </summary>
    public interface IServiceLocatorLifetime : IAsyncLifetime
    {
        IServiceCollection Services { get; }

        Task IAsyncLifetime.InitializeAsync()
        {
            ServiceLocator.SetScopedServiceProvider(Services.BuildServiceProvider());
            return Task.CompletedTask;
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            ServiceLocator.SetScopedServiceProvider(null);
            return Task.CompletedTask;
        }
    }
}
