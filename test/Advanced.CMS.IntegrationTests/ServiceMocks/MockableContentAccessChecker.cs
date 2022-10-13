using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Security;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableContentAccessChecker : ContentAccessChecker
{
    public bool Enabled { get; set; }

    public MockableContentAccessChecker(IContentLoader contentLoader, IPrincipalAccessor principalAccessor) : base(
        contentLoader, principalAccessor)
    {
    }

    public override bool HasSufficientAccess(ContentProvider provider, ContentReference contentLink, AccessLevel access)
    {
        return Enabled || base.HasSufficientAccess(provider, contentLink, access);
    }
}
