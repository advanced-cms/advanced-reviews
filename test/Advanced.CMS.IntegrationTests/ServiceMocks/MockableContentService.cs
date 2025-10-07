using System.Security.Principal;
using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest.Approvals.Internal;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableContentService(
    IContentVersionRepository contentVersionRepository,
    ILanguageBranchRepository languageBranchRepository,
    IContentProviderManager contentProviderManager,
    AncestorReferencesLoader ancestorReferencesLoader,
    LanguageSelectorFactory languageSelectorFactory,
    ISiteDefinitionRepository siteDefinitionRepository,
    ContentEvents contentEvents,
    UIOptions uiOptions,
    IStatusTransitionEvaluator statusTransitionEvaluator,
    ApprovalService approvalService,
    SaveActionRuleEngine ruleEngine)
    : ContentService(ServiceLocator.Current.GetInstance<MockableContentRepository>(),
        contentVersionRepository, languageBranchRepository,
        contentProviderManager, ancestorReferencesLoader, languageSelectorFactory, siteDefinitionRepository,
        ServiceLocator.Current.GetInstance<MockableProjectLoaderService>(), contentEvents, uiOptions,
        statusTransitionEvaluator, approvalService, ruleEngine)
{
    public bool Enabled { get; set; }

    public override bool HasEditAccess(IContent content, AccessLevel accessLevel)
    {
        return Enabled || base.HasEditAccess(content, accessLevel);
    }

    public override bool HasEditAccess(IContent content, IPrincipal principal, AccessLevel accessLevel)
    {
        return Enabled || base.HasEditAccess(content, principal, accessLevel);
    }
}
