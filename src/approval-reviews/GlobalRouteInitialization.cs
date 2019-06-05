using AdvancedApprovalReviews.AvatarsService;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AdvancedApprovalReviews
{
    [InitializableModule]
    public class GlobalRouteInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        public void Initialize(InitializationEngine context)
        {
            Global.RoutesRegistrating += Global_RoutesRegistrating;
        }

        public void Uninitialize(InitializationEngine context)
        {
            Global.RoutesRegistrating -= Global_RoutesRegistrating;
        }

        private void Global_RoutesRegistrating(object sender, EPiServer.Web.Routing.RouteRegistrationEventArgs e)
        {
            e.Routes.RegisterReviewAvatarsRoute();
        }
    }
}
