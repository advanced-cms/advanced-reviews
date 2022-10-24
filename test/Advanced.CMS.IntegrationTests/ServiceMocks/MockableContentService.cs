using System.Security.Principal;
using EPiServer;
using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest.Approvals;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

[ServiceConfiguration(typeof (ContentService))]
[ServiceConfiguration(typeof (MockableContentService))]
[ServiceConfiguration(typeof (ContentService), Lifecycle = ServiceInstanceScope.Singleton)]
public class MockableContentService : ContentService
{
    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    public MockableContentService(IContentRepository contentRepository, IContentVersionRepository contentVersionRepository, ILanguageBranchRepository languageBranchRepository, IContentProviderManager contentProviderManager, AncestorReferencesLoader ancestorReferencesLoader, LanguageSelectorFactory languageSelectorFactory, ISiteDefinitionRepository siteDefinitionRepository, ProjectLoaderService projectLoaderService, ContentEvents contentEvents, UIOptions uiOptions, IStatusTransitionEvaluator statusTransitionEvaluator, ApprovalService approvalService, SaveActionRuleEngine ruleEngine) : base(contentRepository, contentVersionRepository, languageBranchRepository, contentProviderManager, ancestorReferencesLoader, languageSelectorFactory, siteDefinitionRepository, projectLoaderService, contentEvents, uiOptions, statusTransitionEvaluator, approvalService, ruleEngine)
    {
    }

    public override bool HasEditAccess(IContent content, AccessLevel accessLevel)
    {
        return Enabled || base.HasEditAccess(content, accessLevel);
    }

    public override bool HasEditAccess(IContent content, IPrincipal principal, AccessLevel accessLevel)
    {
        return Enabled || base.HasEditAccess(content, principal, accessLevel);
    }
}
