using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

[Collection(IntegrationTestCollection.Name)]
public class Never_Published_Page(Never_Published_Page.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<Never_Published_Page.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        public IContentRepository ContentRepository { get; } = siteFixture.Services.GetInstance<IContentRepository>();
        public IUrlResolver UrlResolver { get; } = siteFixture.Services.GetInstance<IUrlResolver>();
        public IServiceProvider Services => siteFixture.Services;
        public HttpClient Client { get; } = siteFixture.Client;

        public StandardPage Page { get; private set; }
        public ExternalReviewLink GeneratedReviewLink { get; set; }

        public async Task InitializeAsync()
        {
            Page = ContentRepository.CreatePage().WithoutEveryoneAccess();
            GeneratedReviewLink = Page.GenerateExternalReviewLink();

            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => await ContentRepository.CleanupAsync(Page);
    }

    [Fact]
    public async Task Standard_Link_Returns_404()
    {
        var url = fixture.UrlResolver.GetUrl(fixture.Page);
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Review_Link_Returns_200()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.GeneratedReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Review_Link_Page_Is_Rendered()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.GeneratedReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.Contains(fixture.Page.PageName, responseText);
    }
}

