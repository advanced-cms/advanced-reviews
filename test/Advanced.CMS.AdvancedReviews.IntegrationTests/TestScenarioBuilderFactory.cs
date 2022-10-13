using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using Advanced.CMS.IntegrationTests.ServiceMocks;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using TestSite;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[ServiceConfiguration]
public class TestScenarioBuilderFactory
{
    public const string UpdatedString = "_UPDATED";

    private readonly IContentRepository _contentRepository;
    private readonly IContentSecurityRepository _contentSecurityRepository;
    private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
    private readonly ProjectRepository _projectRepository;
    private readonly IContentVersionRepository _contentVersionRepository;
    private readonly ProjectService _projectService;
    private readonly MockableCurrentProject _currentProject;
    private readonly MockableContentService _contentService;
    private readonly MockableContentAccessChecker _contentAccessChecker;

    public TestScenarioBuilderFactory(IContentRepository contentRepository,
        IContentSecurityRepository contentSecurityRepository,
        IExternalReviewLinksRepository externalReviewLinksRepository, ProjectRepository projectRepository,
        MockableCurrentProject currentProject, ProjectService projectService,
        MockableContentService contentService, IContentVersionRepository contentVersionRepository,
        MockableContentAccessChecker contentAccessChecker)
    {
        _contentRepository = contentRepository;
        _contentSecurityRepository = contentSecurityRepository;
        _externalReviewLinksRepository = externalReviewLinksRepository;
        _projectRepository = projectRepository;
        _currentProject = currentProject;
        _projectService = projectService;
        _contentService = contentService;
        _contentVersionRepository = contentVersionRepository;
        _contentAccessChecker = contentAccessChecker;
    }

    public TestScenarioBuilder GetBuilder(TestEnvironment testEnvironment = null)
    {
        var builder = new TestScenarioBuilder(_contentRepository, _contentSecurityRepository,
            _externalReviewLinksRepository, _projectRepository, _currentProject, _projectService,
            _contentVersionRepository);
        if (testEnvironment != null)
        {
            builder.Initialize(testEnvironment);
        }

        return builder;
    }
}
