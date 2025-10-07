using System.Net;
using System.Security.Principal;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Authorization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class When_Creating_Token_For_Page_Without_Everyone_Access(When_Creating_Token_For_Page_Without_Everyone_Access.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<When_Creating_Token_For_Page_Without_Everyone_Access.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        public IContentRepository ContentRepository { get; } = siteFixture.Services.GetInstance<IContentRepository>();
        public IPrincipalAccessor PrincipalAccessor { get; } = siteFixture.Services.GetInstance<IPrincipalAccessor>();
        public IServiceProvider Services => siteFixture.Services;
        public HttpClient Client { get; } = siteFixture.Client;

        public ContentReference TargetFolder { get; private set; }
        public StandardPage Page { get; private set; }
        public string InitialUsername = "Bolek";
        public ExternalReviewLink GeneratedReviewLink { get; set; }

        public async Task InitializeAsync()
        {
            PrincipalAccessor.Principal = new GenericPrincipal(
                new GenericIdentity(InitialUsername),
                [Roles.Administrators, Roles.CmsAdmins, Roles.CmsEditors, Roles.WebAdmins, Roles.WebEditors]);

            TargetFolder = ContentRepository.CreateTargetFolder();
            Page = ContentRepository.CreatePage().WithoutEveryoneAccess();
            GeneratedReviewLink = Page.GenerateExternalReviewLink();

            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => await ContentRepository.CleanupAsync(Page);
    }

    [Fact]
    public async Task It_Returns_200()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.GeneratedReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

