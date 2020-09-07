using System.Web.Routing;

namespace AdvancedApprovalReviews.AvatarsService
{
    public static class AvatarRouteExtensions
    {
        public static void RegisterReviewAvatarsRoute(this RouteCollection routes)
        {
            var route = new Route("review-avatars/get", new AvatarRouteRouteHandler());
            string[] allowedMethods = { "GET" };
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary { { "httpMethod", methodConstraints } };
            routes.Add(route);
        }
    }
}
