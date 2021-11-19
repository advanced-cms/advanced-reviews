using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview
{
    /// <summary>
    /// Register ContentArea draft preview
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DraftContentAreaPreviewInitializerInitializer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // Intercepted to rewrite urls of content items which belong to the same project
            //TODO: NETCORE: fix url resolver
            // context.Services.Intercept<IUrlResolver>(
            //     (locator, defaultUrlResolver) =>
            //         new PreviewUrlResolver(defaultUrlResolver, locator.GetInstance<IContentLoader>(),
            //             locator.GetInstance<IPermanentLinkMapper>(), locator.GetInstance<IContentProviderManager>()));

            // Intercepted in order to return unpublished children in external review context
            context.Services.Intercept<IContentLoader>(
                (locator, defaultContentLoader) => new DraftContentLoader(defaultContentLoader,
                    locator.GetInstance<ServiceAccessor<ReviewsContentLoader>>(),
                    locator.GetInstance<ExternalReviewState>()));

            // Intercepted in order to return unpublished children in external review context
            context.Services.Intercept<ContentLoader>(
                (locator, defaultContentLoader) => new DraftContentLoader(defaultContentLoader,
                    locator.GetInstance<ServiceAccessor<ReviewsContentLoader>>(),
                    locator.GetInstance<ExternalReviewState>()));

            // Intercepted in order to return unpublished content items
            context.Services.Intercept<IPublishedStateAssessor>(
                (locator, defaultPublishedStateAssessor) =>
                    new PublishedStateAssessorDecorator(defaultPublishedStateAssessor,
                        locator.GetInstance<LanguageResolver>(), locator.GetInstance<ExternalReviewState>()));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
