using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Approvals;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

[ServiceConfiguration(typeof (ProjectService))]
[ServiceConfiguration(typeof (MockableProjectService))]
public class MockableProjectService : ProjectService
{
    public override int? CurrentProject => this.CurrentProjectResolver.ProjectId;

    public MockableCurrentProject CurrentProjectResolver { get ; set ; }

    public override bool IsInProjectMode => this.CurrentProject != null;

    public MockableProjectService(ProjectRepository projectRepository, ProjectPublisher projectPublisher,
        ContentService contentService, IContentChangeManager contentChangeManager,
        LanguageSelectorFactory languageSelectorFactory, CurrentProject currentProject,
        ISiteConfigurationRepository siteConfigurationRepository, ApprovalService approvalService,
        LocalizationService localizationService, ProjectUIOptions projectUIOptions) : base(projectRepository,
        projectPublisher, ServiceLocator.Current.GetInstance<MockableContentService>(), contentChangeManager, languageSelectorFactory, ServiceLocator.Current.GetInstance<MockableCurrentProject>(),
        siteConfigurationRepository, approvalService, localizationService, projectUIOptions)
    {
        this.CurrentProjectResolver = ServiceLocator.Current.GetInstance<MockableCurrentProject>();
    }
}
