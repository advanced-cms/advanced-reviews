using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace AdvancedExternalReviews.EditReview
{
    [ServiceConfiguration(typeof(IPartialRouter))]
    public class PageEditPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageEditPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver, IHttpContextAccessor httpContextAccessor)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _httpContextAccessor = httpContextAccessor;
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
                _httpContextAccessor.HttpContext.Items["ImpersonatedVisitorGroupsById"] = externalReviewLink.VisitorGroups;
            }

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingPath = nextSegment.Remaining;

                // We can't set the Edit context here because it breaks the routing if you have useClaims=true in virtualRoles setting
                // and when you have a custom VirtualRole class that uses IPageRouteHelper to fetch the current language from url
                // segmentContext.ContextMode = ContextMode.Edit;
                // TODO: NETCORE segmentContext.RouteData.DataTokens[RoutingConstants.NodeKey] = page.ContentLink;
                segmentContext.Content = page;

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
