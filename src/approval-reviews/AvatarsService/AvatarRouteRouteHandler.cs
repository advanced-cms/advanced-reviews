using System.Web;
using System.Web.Routing;

namespace AdvancedApprovalReviews.AvatarsService
{
    public class AvatarRouteRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ReviewAvatarsHandler();
        }
    }
}
