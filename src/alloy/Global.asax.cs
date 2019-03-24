using System.Web.Routing;
using AdvancedApprovalReviews.AvatarsService;

namespace AlloyTemplates
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected override void OnRoutesRegistrating(RouteCollection routes)
        {
            routes.RegisterReviewAvatarsRoute();

            base.OnRoutesRegistrating(routes);
        }
    }
}
