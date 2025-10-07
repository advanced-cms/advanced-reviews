using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using Advanced.CMS.IntegrationTests.ServiceMocks;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[ServiceConfiguration]
public class TestScenarioBuilderFactory(
    IContentRepository contentRepository,
    IContentSecurityRepository contentSecurityRepository,
    IExternalReviewLinksRepository externalReviewLinksRepository,
    ProjectRepository projectRepository,
    MockableCurrentProject currentProject,
    ProjectService projectService,
    IContentVersionRepository contentVersionRepository)
{
    public TestScenarioBuilder GetBuilder(TestEnvironment testEnvironment = null)
    {
        var builder = new TestScenarioBuilder(contentRepository, contentSecurityRepository,
            externalReviewLinksRepository, projectRepository, currentProject, projectService,
            contentVersionRepository);
        if (testEnvironment != null)
        {
            builder.Initialize(testEnvironment);
        }

        return builder;
    }
}
