using AdvancedExternalReviews.EditReview;
using AdvancedExternalReviews.PinCodeSecurity;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

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
                locator.GetInstance<IExternalReviewLinksRepository>(),
                locator.GetInstance<ExternalReviewOptions>(),
                locator.GetInstance<ProjectContentResolver>(),
                locator.GetInstance<IHttpContextAccessor>()
            );
            // TODO: NETCORE RouteTable.Routes.RegisterPartialRouter(editRouter);

            // register view route
            var previewRouter = new PagePreviewPartialRouter
            (
                locator.GetInstance<IExternalReviewLinksRepository>(),
                locator.GetInstance<ExternalReviewOptions>(),
                locator.GetInstance<ProjectContentResolver>(),
                locator.GetInstance<IExternalLinkPinCodeSecurityHandler>(),
                locator.GetInstance<IHttpContextAccessor>()
            );
            // TODO: NETCORE  RouteTable.Routes.RegisterPartialRouter(previewRouter);

        }

        void IInitializableModule.Uninitialize(InitializationEngine context) { }
    }
}
