using System.Web.Routing;

namespace AdvancedApprovalReviews.AvatarsService
{
    public static class AvatarRouteExtensions
    {
        public static void RegisterReviewAvatarsRoute(this RouteCollection routes)
        {
            var route = new Route("reviewavatars/{userName}.jpg", new AvatarRouteRouteHandler());
            string[] allowedMethods = { "GET", "POST" };
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary { { "httpMethod", methodConstraints } };
            routes.Add(route);
        }
    }
}
