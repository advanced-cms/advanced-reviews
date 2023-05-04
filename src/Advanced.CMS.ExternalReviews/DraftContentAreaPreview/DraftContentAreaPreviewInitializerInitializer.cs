using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview
{
    /// <summary>
    /// Register ContentArea draft preview
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class DraftContentAreaPreviewInitializerInitializer : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (_, _) =>
            {
                // Intercepted to rewrite urls of content items which belong to the same project
                context.Services.Intercept<IUrlResolver>(
                    (locator, defaultUrlResolver) =>
                        new PreviewUrlResolver(defaultUrlResolver, locator.GetInstance<IContentLoader>(),
                            locator.GetInstance<IPermanentLinkMapper>(), locator.GetInstance<IContentProviderManager>(),
                            locator.GetInstance<ExternalReviewState>(),
                            locator.GetInstance<ExternalReviewUrlGenerator>(),
                            locator.GetInstance<ISiteDefinitionResolver>()));

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
                        new PublishedStateAssessorDecorator(defaultPublishedStateAssessor, locator.GetInstance<ExternalReviewState>()));

                // Intercepted in order to not filter out content without Everyone access
                context.Services.Intercept<IContentAccessEvaluator>(
                    (locator, defaultContentAccessEvaluator) =>
                        new ContentAccessEvaluatorDecorator(defaultContentAccessEvaluator,
                            locator.GetInstance<ExternalReviewState>()));
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
