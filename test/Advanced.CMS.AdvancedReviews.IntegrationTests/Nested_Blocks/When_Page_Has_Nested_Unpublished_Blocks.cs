using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Nested_Blocks;

[Collection(IntegrationTestCollection.Name)]
public class When_Page_Has_Nested_Unpublished_Blocks(When_Page_Has_Nested_Unpublished_Blocks.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<When_Page_Has_Nested_Unpublished_Blocks.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        public IContentRepository ContentRepository { get; } = siteFixture.Services.GetInstance<IContentRepository>();
        public IUrlResolver UrlResolver { get; } = siteFixture.Services.GetInstance<IUrlResolver>();
        public IServiceProvider Services => siteFixture.Services;
        public HttpClient Client { get; } = siteFixture.Client;

        public StandardPage Page { get; private set; }

        public async Task InitializeAsync()
        {
            Page = ContentRepository.CreatePage().AddNestedBlock();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => await ContentRepository.CleanupAsync(Page);
    }

    [Fact]
    public async Task Unpublished_Page_Is_Not_Accessible()
    {
        var url = fixture.UrlResolver.GetUrl(fixture.Page);
        var originalMessage = new HttpRequestMessage(HttpMethod.Get, url);
        var originalResponse = await fixture.Client.SendAsync(originalMessage);
        Assert.Equal(HttpStatusCode.NotFound, originalResponse.StatusCode);
    }

    [Fact]
    public async Task After_Publishing_Page_Regular_Preview_Does_Not_Show_Unpublished_Blocks_But_Review_Link_Does()
    {
        fixture.Page.PublishPage();

        var url = fixture.UrlResolver.GetUrl(fixture.Page);
        var originalMessage = new HttpRequestMessage(HttpMethod.Get, url);
        var originalResponse = await fixture.Client.SendAsync(originalMessage);
        Assert.Equal(HttpStatusCode.OK, originalResponse.StatusCode);
        var originalResponseText = await originalResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain(StaticTexts.OriginalNestedBlockContent, originalResponseText);

        var reviewLink = fixture.Page.GenerateExternalReviewLink();
        var message = new HttpRequestMessage(HttpMethod.Get, reviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.Contains(StaticTexts.OriginalNestedBlockContent, responseText);
    }
}

