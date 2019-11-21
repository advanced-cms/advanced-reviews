using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
//    //TODO: consider using a custom project resolver used from an interceptor in ExternalReview scenarios
//    public class CustomProjectResolver : IProjectResolver
//    {
//        private readonly IProjectResolver _projectResolver;
//        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
//
//        public CustomProjectResolver(IProjectResolver projectResolver, IExternalReviewLinksRepository externalReviewLinksRepository)
//        {
//            _projectResolver = projectResolver;
//            _externalReviewLinksRepository = externalReviewLinksRepository;
//        }
//
//        public IEnumerable<int> GetCurrentProjects()
//        {
//            if (ExternalReview.IsInExternalReviewContext)
//            {
//                var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(ExternalReview.Token);
//                if (externalReviewLink.ProjectId.HasValue)
//                {
//                    return new[] {externalReviewLink.ProjectId.Value};
//                }
//            }
//
//            return _projectResolver.GetCurrentProjects();
//        }
//    }

    [ServiceConfiguration(ServiceType = typeof(ProjectContentResolver))]
    public class ProjectContentResolver
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectContentResolver(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public ContentReference GetProjectReference(ContentReference publishedReference, int projectId)
        {
            var items = _projectRepository.ListItems(projectId);
            if (items == null)
            {
                return publishedReference;
            }

            var item = items.FirstOrDefault(x => x.ContentLink.ToReferenceWithoutVersion() == publishedReference.ToReferenceWithoutVersion());
            return item == null ? publishedReference : items.FirstOrDefault(x => x.ContentLink.ID == item.ContentLink.ID).ContentLink;
        }
    }
}
