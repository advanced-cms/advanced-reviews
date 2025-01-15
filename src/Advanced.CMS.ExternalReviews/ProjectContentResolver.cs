using System.Collections.Specialized;
using System.Linq;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews;

internal class ProjectContentResolver
{
    private readonly ProjectRepository _projectRepository;

    public ProjectContentResolver(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public ContentReference GetProjectReference(ContentReference contentLink, int projectId, string language)
    {
        var item = _projectRepository
            .GetItems(new[] { contentLink.ToReferenceWithoutVersion() })
            .Where(x => x.ProjectID == projectId && x.Language.Name == language)
            .FirstOrDefault();
        return item?.ContentLink;
    }

    public IContent TryGetProjectPageVersion(ExternalReviewLink externalReviewLink, IContent routedContent, NameValueCollection queryString)
    {
        var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        var contentReference = externalReviewLink.ContentLink;
        var page = contentLoader.Get<IContent>(contentReference);
        // If not Project associate with the link then use the ContentLink stored in DDS
        if (!externalReviewLink.ProjectId.HasValue)
        {
            return page;
        }

        // If the url is not generated, meaning
        if (!PreviewUrlResolver.IsGeneratedForProjectPreview(queryString))
        {
            return page;
        }

        var projectReference = GetProjectReference(routedContent.ContentLink,
            externalReviewLink.ProjectId.Value, page.LanguageBranch());

        if (projectReference != null)
        {
            return contentLoader.Get<PageData>(projectReference);
        }

        if (routedContent.IsPublished())
        {
            return routedContent;
        }

        var unpublished = routedContent.ContentLink.LoadUnpublishedVersion();
        return contentLoader.Get<PageData>(unpublished);
    }
}
