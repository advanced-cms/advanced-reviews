using System.Security.Principal;
using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Security;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableContentLoaderService(
    IContentLoader contentLoader,
    IContentVersionRepository contentVersionRepository,
    ILanguageBranchRepository languageBranchRepository,
    IContentProviderManager contentProviderManager,
    AncestorReferencesLoader ancestorReferencesLoader,
    LanguageSelectorFactory languageSelectorFactory,
    ISiteDefinitionRepository siteDefinitionRepository)
    : ContentLoaderService(contentLoader, contentVersionRepository, languageBranchRepository, contentProviderManager,
        ancestorReferencesLoader, languageSelectorFactory, siteDefinitionRepository)
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
