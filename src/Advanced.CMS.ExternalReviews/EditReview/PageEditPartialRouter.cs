﻿using System.Globalization;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Advanced.CMS.ExternalReviews.EditReview
{
    [ServiceConfiguration(typeof(IPartialRouter))]
    public class PageEditPartialRouter : IPartialRouter<IContent, IContent>
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IContentLanguageAccessor _contentLanguageAccessor;
        private readonly ExternalReviewState _externalReviewState;

        public PageEditPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver, IContentLanguageAccessor contentLanguageAccessor,
            ExternalReviewState externalReviewState)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _contentLanguageAccessor = contentLanguageAccessor;
            _externalReviewState = externalReviewState;
        }

        public PartialRouteData GetPartialVirtualPath(IContent content, UrlGeneratorContext urlGeneratorContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(IContent content, UrlResolverContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled || !_externalReviewOptions.EditableLinksEnabled)
            {
                return null;
            }

            var nextSegment = segmentContext.GetNextSegment(segmentContext.RemainingSegments);
            if (nextSegment.Next.IsEmpty || nextSegment.Next.ToString() != _externalReviewOptions.ContentIframeEditUrlSegment)
            {
                return null;
            }

            nextSegment = segmentContext.GetNextSegment(nextSegment.Remaining);
            var token = nextSegment.Next.ToString();

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return null;
            }

            _externalReviewState.ProjectId = externalReviewLink.ProjectId;
            _externalReviewState.IsEditLink = true;
            _externalReviewState.Token = token;

            _contentLanguageAccessor.Language = new CultureInfo(content.LanguageBranch());

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingSegments = nextSegment.Remaining;

                // We can't set the Edit context here because it breaks the routing if you have useClaims=true in virtualRoles setting
                // and when you have a custom VirtualRole class that uses IPageRouteHelper to fetch the current language from url
                // segmentContext.ContextMode = ContextMode.Edit;
                segmentContext.RouteValues[RoutingConstants.ContentLinkKey] = page.ContentLink;

                return page;
            }
            catch
            {
                return null;
            }
        }
    }
}
