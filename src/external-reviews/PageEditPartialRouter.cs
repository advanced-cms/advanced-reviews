using System;
using System.Web;
using System.Web.Routing;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AdvancedExternalReviews
{
    public class PageEditPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;

        public PageEditPartialRouter(IContentLoader contentLoader, IExternalReviewLinksRepository externalReviewLinksRepository)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return null;
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            if (nextSegment.Next != "externalPageReview")
            {
                return null;
            }

            nextSegment = segmentContext.GetNextValue(nextSegment.Remaining);
            var token = nextSegment.Next;

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return null;
            }
            
            // HttpContext.Current.Request.RequestContext.SetContextMode(ContextMode.Edit);

            try
            {
                var page = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);
                segmentContext.RemainingPath = nextSegment.Remaining;

                segmentContext.ContextMode = ContextMode.Edit;
                ExternalReview.IsInExternalReviewContext = true;

                return page;
            }
            catch (ContentNotFoundException e)
            {
                return null;
            }
        }
    }
}
