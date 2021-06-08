using System.Web.Routing;
using AdvancedApprovalReviews.AvatarsService;

namespace AdvancedExternalReviews.ImageProxy
{
    public static class ImageProxyExtensions
    {
        public const string ImageProxyRoute = "advanced-reviews-image-proxy/get";

        public static void RegisterImageProxyRoute(this RouteCollection routes)
        {
            var routeData = new RouteValueDictionary {{"Controller", nameof(ReviewAvatarsHandler)}};
            var route = new Route(ImageProxyRoute, routeData, new ImageProxyRouteHandler());
            string[] allowedMethods = { "GET" };
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary { { "httpMethod", methodConstraints } };
            routes.Add(route);
        }
    }
}
