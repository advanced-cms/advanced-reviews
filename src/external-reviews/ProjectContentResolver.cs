using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    /// <summary>
    /// Use ProjectLoaderService after upgrading CMS UI dependency to 11.32
    /// </summary>
    [ServiceConfiguration(ServiceType = typeof(ProjectContentResolver))]
    public class ProjectContentResolver
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectContentResolver(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
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
    }
}
