using Advanced.CMS.ApprovalReviews;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews.ReviewLinksRepository
{
    [ServiceConfiguration(typeof(ExternalReviewLinkBuilder))]
    public class ExternalReviewLinkBuilder
    {
        private readonly ProjectRepository _projectRepository;
        private readonly ExternalReviewOptions _options;
        private readonly IStartPageUrlResolver _startPageUrlResolver;

        public ExternalReviewLinkBuilder(ExternalReviewOptions options,
            IStartPageUrlResolver startPageUrlResolver, ProjectRepository projectRepository)
        {
            _options = options;
            _startPageUrlResolver = startPageUrlResolver;
            _projectRepository = projectRepository;
        }

        public ExternalReviewLink FromExternalReview(ExternalReviewLinkDds externalReviewLinkDds)
        {
            var projectName = "";
            var projectId = externalReviewLinkDds.ProjectId;
            if (projectId.HasValue)
            {
                try
                {
                    var project = _projectRepository.Get(externalReviewLinkDds.ProjectId.Value);
                    projectName = project.Name;
                }
                catch
                {
                    projectName = string.Empty;
                    projectId = null;
                }
            }

            string externalUrlPrefix;
            if (externalReviewLinkDds.IsEditable)
            {
                externalUrlPrefix = UrlPath.EnsureStartsWithSlash(_options.ReviewsUrl);
            }
            else
            {
                var url = _startPageUrlResolver.GetUrl(externalReviewLinkDds.ContentLink);
                // the preview url has to be language specific as it's handled entirely by the EPiServer partial router
                // the edit url is just a pure aspnet.mvc controller, handled outside EPiServer
                externalUrlPrefix = UrlPath.Combine(url, _options.ContentPreviewUrl);
            }

            return new ExternalReviewLink
            {
                ContentLink = externalReviewLinkDds.ContentLink,
                IsEditable = externalReviewLinkDds.IsEditable,
                ProjectId = projectId,
                Token = externalReviewLinkDds.Token,
                ValidTo = externalReviewLinkDds.ValidTo,
                LinkUrl = UrlPath.Combine(externalUrlPrefix, externalReviewLinkDds.Token),
                PinCode = externalReviewLinkDds.PinCode,
                DisplayName = externalReviewLinkDds.DisplayName,
                ProjectName = projectName,
                VisitorGroups = externalReviewLinkDds.VisitorGroups
            };
        }
    }
}
