using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

[ServiceConfiguration(typeof (ProjectLoaderService))]
[ServiceConfiguration(typeof (MockableProjectLoaderService))]
public class MockableProjectLoaderService : ProjectLoaderService
{
    public override int? CurrentProject => this.CurrentProjectResolver.ProjectId;

    public MockableProjectLoaderService(ProjectRepository projectRepository, CurrentProject currentProject, ContentLoaderService contentLoaderService, ISiteConfigurationRepository siteConfigurationRepository, ProjectUIOptions projectUIOptions) : base(projectRepository, currentProject, contentLoaderService, siteConfigurationRepository, projectUIOptions)
    {
        this.CurrentProjectResolver = ServiceLocator.Current.GetInstance<MockableCurrentProject>();
    }

    public MockableCurrentProject CurrentProjectResolver { get ; set ; }

    public override bool IsInProjectMode => this.CurrentProject != null;
}
