using System.Web;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AdvancedExternalReviews
{
    public class PagePreviewPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IContentLoader _contentLoader;

        public PagePreviewPartialRouter(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return null;
            /*   return new PartialRouteData()
               {
                   BasePathRoot = ContentReference.StartPage,
                   PartialVirtualPath = $"externalPageReview/{content.ContentLink}/"
               };*/
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            if (nextSegment.Next != "externalPageReview")
            {
                return null;
            }

            nextSegment = segmentContext.GetNextValue(nextSegment.Remaining);
            if (!ContentReference.TryParse(nextSegment.Next, out var reference))
            {
                return null;
            }

            HttpContext.Current.Request.RequestContext.SetContextMode(ContextMode.Edit);

            //segmentContext.RouteData.DataTokens[RoutingConstants.ContextModeKey] = ContextMode.Edit;
            var page = _contentLoader.Get<PageData>(reference);
            segmentContext.RemainingPath = nextSegment.Remaining;

            segmentContext.ContextMode = ContextMode.Edit;
            //segmentContext.RouteData.DataTokens[RoutingConstants.ContextModeKey] = ContextMode.Edit;

            return page;
        }
    }
}
