using System.Web.Routing;
using AdvancedExternalReviews.EditReview;
using AdvancedExternalReviews.PinCodeSecurity;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PreviewRouterInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // register edit route
            var locator = context.Locate.Advanced;
            var editRouter = new PageEditPartialRouter
            (
                locator.GetInstance<IContentLoader>(),
                locator.GetInstance<IExternalReviewLinksRepository>(),
                locator.GetInstance<ExternalReviewOptions>()
            );
            RouteTable.Routes.RegisterPartialRouter(editRouter);

            // register view route
            var previewRouter = new PagePreviewPartialRouter
            (
                locator.GetInstance<IContentLoader>(),
                locator.GetInstance<IExternalReviewLinksRepository>(),
                locator.GetInstance<ExternalReviewOptions>(),
                locator.GetInstance<ProjectContentResolver>(),
                locator.GetInstance<IExternalLinkPinCodeSecurityHandler>()
            );
            RouteTable.Routes.RegisterPartialRouter(previewRouter);

        }

        void IInitializableModule.Uninitialize(InitializationEngine context) { }
    }
}
