using System.Web.Routing;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Core;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace AdvancedExternalReviews.EditReview
{
    public class PageEditPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly ProjectContentResolver _projectContentResolver;

        public PageEditPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled || !_externalReviewOptions.EditableLinksEnabled)
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

            if (externalReviewLink.VisitorGroups != null)
            {
                System.Web.HttpContext.Current.Items["ImpersonatedVisitorGroupsById"] = externalReviewLink.VisitorGroups;
            }

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.QueryString);

                segmentContext.RemainingPath = nextSegment.Remaining;

                // We can't set the Edit context here because it breaks the routing if you have useClaims=true in virtualRoles setting
                // and when you have a custom VirtualRole class that uses IPageRouteHelper to fetch the current language from url
                // segmentContext.ContextMode = ContextMode.Edit;
                segmentContext.RouteData.DataTokens[RoutingConstants.NodeKey] = page.ContentLink;

                ExternalReview.ProjectId = externalReviewLink.ProjectId;
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
