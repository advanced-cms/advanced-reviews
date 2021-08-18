using System;
using AdvancedExternalReviews.PinCodeSecurity;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AdvancedExternalReviews
{
    /// <summary>
    /// Partial router used to display readonly version of the page
    /// </summary>
    public class PagePreviewPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagePreviewPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver,
            IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler, IHttpContextAccessor httpContextAccessor)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, UrlGeneratorContext segmentContext)
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
            if (string.IsNullOrWhiteSpace(nextSegment.Next))
            {
                return null;
            }

            if (!string.Equals(nextSegment.Next, _externalReviewOptions.ContentPreviewUrl, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            nextSegment = segmentContext.GetNextRemainingSegment(nextSegment.Remaining);
            var token = nextSegment.Next;

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsPreviewableLink())
            {
                return null;
            }

            if (externalReviewLink.VisitorGroups != null)
            {
                _httpContextAccessor.HttpContext.Items["ImpersonatedVisitorGroupsById"] = externalReviewLink.VisitorGroups;
            }

            // PIN code security check, if user is not authenticated, then redirect to login page
            if (!_externalLinkPinCodeSecurityHandler.UserHasAccessToLink(externalReviewLink))
            {
                _externalLinkPinCodeSecurityHandler.RedirectToLoginPage(externalReviewLink);
                return null;
            }

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingPath = nextSegment.Remaining;

                // set ContentLink in DataTokens to make IPageRouteHelper working
                // TODO: NETCORE segmentContext.RouteData.DataTokens[RoutingConstants.NodeKey] = page.ContentLink;
                segmentContext.Content = page;
                ExternalReview.Token = token;
                ExternalReview.ProjectId = externalReviewLink.ProjectId;

                return page;
            }
            catch
            {
                return null;
            }
        }
    }
}
