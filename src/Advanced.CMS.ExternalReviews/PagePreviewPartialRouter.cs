using System;
using System.Globalization;
using Advanced.CMS.ExternalReviews.PinCodeSecurity;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Advanced.CMS.ExternalReviews
{
    /// <summary>
    /// Partial router used to display readonly version of the page
    /// </summary>
    [ServiceConfiguration(typeof(IPartialRouter))]
    public class PagePreviewPartialRouter : IPartialRouter<IContent, IContent>
    {
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly IContentLanguageAccessor _contentLanguageAccessor;
        private readonly ExternalReviewState _externalReviewState;
        private readonly IContentVersionRepository _contentVersionRepository;

        public PagePreviewPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver,
            IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler,
            IContentLanguageAccessor contentLanguageAccessor, ExternalReviewState externalReviewState,
            IContentVersionRepository contentVersionRepository)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
            _contentLanguageAccessor = contentLanguageAccessor;
            _externalReviewState = externalReviewState;
            _contentVersionRepository = contentVersionRepository;
        }

        public PartialRouteData GetPartialVirtualPath(IContent content, UrlGeneratorContext segmentContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(IContent content, UrlResolverContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled)
            {
                return null;
            }

            var nextSegment = segmentContext.GetNextSegment(segmentContext.RemainingSegments);
            if (nextSegment.Next.IsEmpty)
            {
                return null;
            }

            if (!string.Equals(nextSegment.Next.ToString(), _externalReviewOptions.ContentPreviewUrl,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            nextSegment = segmentContext.GetNextSegment(nextSegment.Remaining);
            var token = nextSegment.Next.ToString();

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsPreviewableLink())
            {
                return null;
            }

            var version = _contentVersionRepository.Load(externalReviewLink.ContentLink);
            _contentLanguageAccessor.Language = new CultureInfo(version.LanguageBranch);
            _externalReviewState.Token = token;
            _externalReviewState.ProjectId = externalReviewLink.ProjectId;
            _externalReviewState.PreferredLanguage = version.LanguageBranch;

            // PIN code security check, if user is not authenticated, then redirect to login page
            if (!_externalLinkPinCodeSecurityHandler.UserHasAccessToLink(externalReviewLink))
            {
                _externalLinkPinCodeSecurityHandler.RedirectToLoginPage(externalReviewLink);
                return null;
            }

            try
            {
                var versionedContent = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingSegments = nextSegment.Remaining;

                // set ContentLink in DataTokens to make IPageRouteHelper working
                segmentContext.RouteValues[RoutingConstants.ContentLinkKey] = versionedContent.ContentLink;

                return versionedContent;
            }
            catch
            {
                return null;
            }
        }
    }
}
