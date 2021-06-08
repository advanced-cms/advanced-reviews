using System.Web;
using System.Web.Routing;

namespace AdvancedExternalReviews.ImageProxy
{
    public class ImageProxyRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImageProxyHandler();
        }
    }
}
