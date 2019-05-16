using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;

namespace AdvancedExternalReviews.DraftContentAreaPreview
{
    /// <summary>
    /// Register ContentArea draft preview
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DrafContentAreaPreviewInitializerInitializer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Intercept<IContentAreaLoader>(
                (locator, defaultContentAreaLoader) => new DraftContentAreaLoader(defaultContentAreaLoader,
                    locator.GetInstance<IContentLoader>(), locator.GetInstance<LanguageResolver>(),
                    locator.GetInstance<IContentVersionRepository>()));


            context.Services.Intercept<ContentAreaRenderer>(
                (locator, defaultContentAreaRenderer) => new DraftContentAreaRenderer(defaultContentAreaRenderer));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
