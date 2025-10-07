using EPiServer.Core.Internal;
using EPiServer.Security;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableContentAccessChecker(IContentLoader contentLoader, IPrincipalAccessor principalAccessor)
    : ContentAccessChecker(contentLoader, principalAccessor)
{
    public bool Enabled { get; set; }

    public override bool HasSufficientAccess(ContentProvider provider, ContentReference contentLink, AccessLevel access)
    {
        return Enabled || base.HasSufficientAccess(provider, contentLink, access);
    }
}
