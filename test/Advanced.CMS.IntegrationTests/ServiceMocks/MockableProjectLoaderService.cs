using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableProjectLoaderService : ProjectLoaderService
{
    public MockableProjectLoaderService(ProjectRepository projectRepository,
        ISiteConfigurationRepository siteConfigurationRepository,
        ProjectUIOptions projectUIOptions) : base(projectRepository,
        ServiceLocator.Current.GetInstance<MockableCurrentProject>(),
        ServiceLocator.Current.GetInstance<MockableContentLoaderService>(),
        siteConfigurationRepository, projectUIOptions)
    {
    }
}
