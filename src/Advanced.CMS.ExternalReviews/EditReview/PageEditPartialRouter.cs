using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.ExternalReviews.EditReview
{
    [ServiceConfiguration(typeof(IPartialRouter))]
    public class PageEditPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExternalReviewState _externalReviewState;

        public PageEditPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver, IHttpContextAccessor httpContextAccessor,
            ExternalReviewState externalReviewState)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _httpContextAccessor = httpContextAccessor;
            _externalReviewState = externalReviewState;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, UrlGeneratorContext urlGeneratorContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(PageData content, UrlResolverContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled)
            {
                return null;
            }

            var nextSegment = segmentContext.GetNextRemainingSegment(segmentContext.RemainingPath);
            if (nextSegment.Next != _externalReviewOptions.ContentIframeEditUrlSegment)
            {
                return null;
            }

            nextSegment = segmentContext.GetNextRemainingSegment(nextSegment.Remaining);
            var token = nextSegment.Next;

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return null;
            }

            if (externalReviewLink.VisitorGroups != null)
            {
                _httpContextAccessor.HttpContext.Items["ImpersonatedVisitorGroupsById"] =
                    externalReviewLink.VisitorGroups;
            }

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingPath = nextSegment.Remaining;

                // We can't set the Edit context here because it breaks the routing if you have useClaims=true in virtualRoles setting
                // and when you have a custom VirtualRole class that uses IPageRouteHelper to fetch the current language from url
                // segmentContext.ContextMode = ContextMode.Edit;
                segmentContext.RouteValues[RoutingConstants.ContentLinkKey] = page.ContentLink;
                segmentContext.RouteValues["module"] =

                _externalReviewState.ProjectId = externalReviewLink.ProjectId;
                _externalReviewState.IsEditLink = true;
                _externalReviewState.Token = token;

                return page;
            }
            catch
            {
                return null;
            }
        }
    }
}
