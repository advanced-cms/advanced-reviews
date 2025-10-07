using System.Globalization;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.Web.Routing;
using Microsoft.Extensions.Options;

namespace Advanced.CMS.ExternalReviews.EditReview;

internal class PageEditPartialRouter(
    IExternalReviewLinksRepository externalReviewLinksRepository,
    IOptions<ExternalReviewOptions> externalReviewOptions,
    ProjectContentResolver projectContentResolver,
    IContentLanguageAccessor contentLanguageAccessor,
    ExternalReviewState externalReviewState,
    IContentVersionRepository contentVersionRepository)
    : IPartialRouter<IContent, IContent>
{
    public PartialRouteData GetPartialVirtualPath(IContent content, UrlGeneratorContext urlGeneratorContext)
    {
        return new PartialRouteData();
    }

    public object RoutePartial(IContent content, UrlResolverContext segmentContext)
    {
        if (!externalReviewOptions.Value.IsEnabled || !externalReviewOptions.Value.EditableLinksEnabled)
        {
            return null;
        }

        var nextSegment = segmentContext.GetNextSegment(segmentContext.RemainingSegments);
        if (nextSegment.Next.IsEmpty || !nextSegment.Next.ToString().Equals(externalReviewOptions.Value.ContentIframeEditUrlSegment, StringComparison.CurrentCultureIgnoreCase))
        {
            return null;
        }

        nextSegment = segmentContext.GetNextSegment(nextSegment.Remaining);
        var token = nextSegment.Next.ToString();

        var externalReviewLink = externalReviewLinksRepository.GetContentByToken(token);
        if (!externalReviewLink.IsEditableLink())
        {
            return null;
        }

        var version = contentVersionRepository.Load(externalReviewLink.ContentLink);
        contentLanguageAccessor.Language = new CultureInfo(version.LanguageBranch);

        externalReviewState.ProjectId = externalReviewLink.ProjectId;
        externalReviewState.IsEditLink = true;
        externalReviewState.Token = token;
        externalReviewState.PreferredLanguage = version.LanguageBranch;
        externalReviewState.ImpersonatedVisitorGroupsById = externalReviewLink.VisitorGroups;

        try
        {
            var page = projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
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
