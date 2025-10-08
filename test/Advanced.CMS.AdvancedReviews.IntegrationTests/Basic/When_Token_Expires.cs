using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.ServiceLocation;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

[Collection(IntegrationTestCollection.Name)]
public class When_Token_Expires(When_Token_Expires.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<When_Token_Expires.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        public IContentRepository ContentRepository { get; } = siteFixture.Services.GetInstance<IContentRepository>();
        public IServiceProvider Services => siteFixture.Services;
        public HttpClient Client { get; } = siteFixture.Client;

        public StandardPage Page { get; private set; }
        public ExternalReviewLink GeneratedReviewLink { get; set; }

        public async Task InitializeAsync()
        {
            Page = ContentRepository.CreatePage().WithoutEveryoneAccess();
            GeneratedReviewLink = Page.GenerateExternalReviewLink().ExpireReviewLink();

            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => await ContentRepository.CleanupAsync(Page);
    }

    [Fact]
    public async Task It_Returns_200()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.GeneratedReviewLink.LinkUrl);
        var responseAfterTokenExpiration = await fixture.Client.SendAsync(message);
        await responseAfterTokenExpiration.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, responseAfterTokenExpiration.StatusCode);
    }
}

