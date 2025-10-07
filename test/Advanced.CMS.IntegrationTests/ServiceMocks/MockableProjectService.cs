using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Approvals.Internal;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Data;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableProjectService(
    ProjectRepository projectRepository,
    ProjectPublisher projectPublisher,
    IContentChangeManager contentChangeManager,
    LanguageSelectorFactory languageSelectorFactory,
    ISiteConfigurationRepository siteConfigurationRepository,
    ApprovalService approvalService,
    LocalizationService localizationService,
    ProjectUIOptions projectUiOptions,
    IDatabaseMode databaseMode)
    : ProjectService(projectRepository,
        projectPublisher, ServiceLocator.Current.GetInstance<MockableContentService>(), contentChangeManager,
        languageSelectorFactory, ServiceLocator.Current.GetInstance<MockableCurrentProject>(),
        siteConfigurationRepository, approvalService, localizationService, projectUiOptions, databaseMode);
