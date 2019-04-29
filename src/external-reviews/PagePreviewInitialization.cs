using System.Web.Routing;
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
            var locator = context.Locate.Advanced;
            var partialRouter = new PagePreviewPartialRouter
            (
                locator.GetInstance<IContentLoader>()
            );
            RouteTable.Routes.RegisterPartialRouter(partialRouter);
        }

        void IInitializableModule.Uninitialize(InitializationEngine context) { }
    }
}
