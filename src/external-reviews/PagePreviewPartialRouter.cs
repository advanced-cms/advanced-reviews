using System;
using System.Web.Routing;
using AdvancedExternalReviews.PinCodeSecurity;
using AdvancedExternalReviews.ReviewLinksRepository;
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
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public PagePreviewPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver,
            IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            if (!_externalReviewOptions.IsPublicPreviewEnabled)
            {
                return null;
            }

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

            if (externalReviewLink.VisitorGroups != null)
            {
                System.Web.HttpContext.Current.Items["ImpersonatedVisitorGroupsById"] = externalReviewLink.VisitorGroups;
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
                    segmentContext.QueryString);

                segmentContext.RemainingPath = nextSegment.Remaining;

                // set ContentLink in DataTokens to make IPageRouteHelper working
                segmentContext.RouteData.DataTokens[RoutingConstants.NodeKey] = page.ContentLink;
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
