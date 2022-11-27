using System.Collections.Specialized;
using System.Linq;
using Advanced.CMS.ExternalReviews.DraftContentAreaPreview;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews
{
    /// <summary>
    /// Use ProjectLoaderService after upgrading CMS UI dependency to 11.32
    /// </summary>
    [ServiceConfiguration(ServiceType = typeof(ProjectContentResolver))]
    public class ProjectContentResolver
    {
        private readonly ProjectRepository _projectRepository;
        private readonly IContentLoader _contentLoader;

        public ProjectContentResolver(ProjectRepository projectRepository, IContentLoader contentLoader)
        {
            _projectRepository = projectRepository;
            _contentLoader = contentLoader;
        }

        public ContentReference GetProjectReference(ContentReference contentLink, int projectId, string language)
        {
            var items = _projectRepository.ListItems(projectId);

            var item = items.FirstOrDefault(
                x => x.ContentLink.ToReferenceWithoutVersion() == contentLink.ToReferenceWithoutVersion()
                     && (x.Language?.Name == language)
            );
            return item?.ContentLink;
        }

        public IContent TryGetProjectPageVersion(ExternalReviewLink externalReviewLink, IContent routedContent, NameValueCollection queryString)
        {
            var contentReference = externalReviewLink.ContentLink;
            var page = _contentLoader.Get<IContent>(contentReference);
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
                return _contentLoader.Get<PageData>(projectReference);
            }

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            if (routedContent.IsPublished())
            {
                return routedContent;
            }

            var unpublished = reviewsContentLoader.LoadUnpublishedVersion(routedContent.ContentLink);
            return _contentLoader.Get<PageData>(unpublished);
        }
    }
}
