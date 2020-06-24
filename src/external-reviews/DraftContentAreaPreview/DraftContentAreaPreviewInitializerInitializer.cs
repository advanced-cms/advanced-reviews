using EPiServer;
using EPiServer.Core.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews.DraftContentAreaPreview
{
    /// <summary>
    /// Register ContentArea draft preview
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DraftContentAreaPreviewInitializerInitializer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Intercept<IContentAreaLoader>(
                (locator, defaultContentAreaLoader) => new DraftContentAreaLoader(defaultContentAreaLoader,
                    locator.GetInstance<IContentLoader>(), locator.GetInstance<ReviewsContentLoader>()));

            context.Services.Intercept<ContentAreaRenderer>(
                (locator, defaultContentAreaRenderer) => new DraftContentAreaRenderer(defaultContentAreaRenderer));

            context.Services.Intercept<UrlResolver>(
                (locator, defaultUrlResolver) =>
                    new PreviewUrlResolver(defaultUrlResolver, locator.GetInstance<IContentLoader>(),
                        locator.GetInstance<IPermanentLinkMapper>()));

            context.Services.Intercept<IContentLoader>(
                (locator, defaultContentLoader) =>
                {
                    if (!locator.GetInstance<ExternalReviewOptions>().ContentReplacement.ReplaceChildren)
                    {
                        return defaultContentLoader;
                    }

                    return new DraftContentLoader(defaultContentLoader, locator.GetInstance<ServiceAccessor<ReviewsContentLoader>>());
                });

            context.Services.Intercept<ContentLoader>(
                (locator, defaultContentLoader) =>
                {
                    if (!locator.GetInstance<ExternalReviewOptions>().ContentReplacement.ReplaceChildren)
                    {
                        return defaultContentLoader;
                    }

                    return new DraftContentLoader(defaultContentLoader, locator.GetInstance<ServiceAccessor<ReviewsContentLoader>>());
                });
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
