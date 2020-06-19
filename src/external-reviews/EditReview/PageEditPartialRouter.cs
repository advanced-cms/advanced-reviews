﻿using System.Web.Routing;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AdvancedExternalReviews.EditReview
{
    public class PageEditPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public PageEditPartialRouter(IContentLoader contentLoader, IExternalReviewLinksRepository externalReviewLinksRepository, ExternalReviewOptions externalReviewOptions)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled)
            {
                return null;
            }

            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            if (nextSegment.Next != _externalReviewOptions.ContentIframeEditUrlSegment)
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

            try
            {
                var page = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);
                segmentContext.RemainingPath = nextSegment.Remaining;

                // We can't set the Edit context here because it breaks the routing if you have useClaims=true in virtualRoles setting
                // and when you have a custom VirtualRole class that uses IPageRouteHelper to fetch the current language from url
                // segmentContext.ContextMode = ContextMode.Edit;
                segmentContext.RouteData.DataTokens[RoutingConstants.NodeKey] = page.ContentLink;

                ExternalReview.IsEditLink = true;
                ExternalReview.Token = token;

                return page;
            }
            catch
            {
                return null;
            }
        }
    }
}
