using System;
using System.Web.Routing;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AdvancedExternalReviews
{
    /// <summary>
    /// Partial router used to display readonly version of the page
    /// </summary>
    public class PagePreviewPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public PagePreviewPartialRouter(IContentLoader contentLoader,
            IExternalReviewLinksRepository externalReviewLinksRepository, ExternalReviewOptions externalReviewOptions)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return null;
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            if (string.IsNullOrWhiteSpace(nextSegment.Next))
            {
                return null;
            }

            if (!string.Equals(nextSegment.Next, _externalReviewOptions.ContentPreviewUrl, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            nextSegment = segmentContext.GetNextValue(nextSegment.Remaining);
            var token = nextSegment.Next;

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsPreviewableLink())
            {
                return null;
            }
            
            try
            {
                var page = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);
                segmentContext.RemainingPath = nextSegment.Remaining;
                ExternalReview.IsInExternalReviewContext = true;

                return page;
            }
            catch (ContentNotFoundException)
            {
                return null;
            }
        }
    }
}
