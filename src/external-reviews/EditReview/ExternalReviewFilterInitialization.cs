using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews.EditReview
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ExternalReviewFilterInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var providers = FilterProviders.Providers.ToList();
            FilterProviders.Providers.Clear();
            FilterProviders.Providers.Add(new ExternalReviewFilterProvider(providers));
        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {

        }
    }
}
