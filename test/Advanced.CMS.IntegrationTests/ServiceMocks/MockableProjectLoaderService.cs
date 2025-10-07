using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Data;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableProjectLoaderService(
    ProjectRepository projectRepository,
    ISiteConfigurationRepository siteConfigurationRepository,
    ProjectUIOptions projectUiOptions,
    IDatabaseMode databaseMode)
    : ProjectLoaderService(projectRepository,
        ServiceLocator.Current.GetInstance<MockableCurrentProject>(),
        ServiceLocator.Current.GetInstance<MockableContentLoaderService>(),
        siteConfigurationRepository, projectUiOptions, databaseMode);
