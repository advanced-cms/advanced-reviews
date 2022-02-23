using System;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.IntegrationTests
{
    [Obsolete("Replace 'IClassFixture<ServiceProviderResolverFixture>' with 'IServiceLocatorLifetime'")]
    public class ServiceProviderResolverFixture : IDisposable
    {
        private IServiceProvider _currentLocator;
        public ServiceCollection Services { get; }
        public ServiceProviderResolverFixture()
        {
            _currentLocator = ServiceLocator.Current;
            Services = new ServiceCollection();
            ServiceLocator.SetScopedServiceProvider(Services.BuildServiceProvider());
        }

        public void ReBuild(IServiceCollection newServices = null)
        {
            ServiceLocator.SetScopedServiceProvider((newServices ?? Services).BuildServiceProvider());
        }
        public void Dispose() => ServiceLocator.SetScopedServiceProvider(_currentLocator);
    }
}
