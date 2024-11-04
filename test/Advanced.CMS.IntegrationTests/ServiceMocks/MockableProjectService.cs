using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Approvals.Internal;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableProjectService : ProjectService
{
    public MockableProjectService(ProjectRepository projectRepository, ProjectPublisher projectPublisher,
        IContentChangeManager contentChangeManager,
        LanguageSelectorFactory languageSelectorFactory,
        ISiteConfigurationRepository siteConfigurationRepository, ApprovalService approvalService,
        LocalizationService localizationService, ProjectUIOptions projectUIOptions, IDatabaseMode databaseMode) : base(
        projectRepository,
        projectPublisher, ServiceLocator.Current.GetInstance<MockableContentService>(), contentChangeManager,
        languageSelectorFactory, ServiceLocator.Current.GetInstance<MockableCurrentProject>(),
        siteConfigurationRepository, approvalService, localizationService, projectUIOptions, databaseMode)
    {
    }
}
