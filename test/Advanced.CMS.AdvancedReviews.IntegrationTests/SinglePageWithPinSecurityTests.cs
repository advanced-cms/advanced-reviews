using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class SinglePageWithPinSecurityTests(SinglePageWithPinSecurityTests.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<SinglePageWithPinSecurityTests.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        private readonly IContentRepository contentRepository = siteFixture.Services.GetService<IContentRepository>();
        public readonly IOptions<ExternalReviewOptions> ExternalReviewOptions = siteFixture.Services.GetService<IOptions<ExternalReviewOptions>>();
        public readonly ServiceAccessor<SiteDefinition> CurrentSiteDefinition = siteFixture.Services.GetService<ServiceAccessor<SiteDefinition>>();
        public HttpClient Client { get; } = siteFixture.Client;

        public StandardPage Page { get; private set; }
        public ExternalReviewLink GeneratedReviewLink { get; set; }
        public string ReviewedPageContent = "test page";
        public string PinCode = "123";

        public async Task InitializeAsync()
        {
            ExternalReviewOptions.Value.PinCodeSecurity.Enabled = true;
            Page = contentRepository.CreatePage(ReviewedPageContent).WithoutEveryoneAccess();

            var reviewLinksRepository =
                siteFixture.Services.GetRequiredService<IExternalReviewLinksRepository>();
            reviewLinksRepository.GetLinksForContent(Page.ContentLink, null);
            var link = reviewLinksRepository.AddLink(Page.ContentLink, false, TimeSpan.FromDays(1), null);
            GeneratedReviewLink = reviewLinksRepository.UpdateLink(link.Token, DateTime.Now.AddDays(1), PinCode, null, null);

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await contentRepository.CleanupAsync(Page);
            ExternalReviewOptions.Value.PinCodeSecurity.Enabled = false;
        }
    }

    [Fact]
    public async Task When_PinSecurity_Enabled_Login_Screen_Is_Shown()
    {
        var siteDefinition = fixture.CurrentSiteDefinition();

        var message = new HttpRequestMessage(HttpMethod.Get, fixture.GeneratedReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        var text = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Go", text);

        var externalLoginUrl = new Uri(siteDefinition.SiteUrl,
            fixture.ExternalReviewOptions.Value.PinCodeSecurity.ExternalReviewLoginUrl);
        var failedLoginMessage = new HttpRequestMessage(HttpMethod.Post,
            externalLoginUrl);
        failedLoginMessage.Content = new FormUrlEncodedContent(new LoginModel
        {
            Code = "111111111111111", Token = fixture.GeneratedReviewLink.Token
        }.AsDictionary<string>());
        var failedLoginResponse = await fixture.Client.SendAsync(failedLoginMessage);
        Assert.Equal(HttpStatusCode.NotFound, failedLoginResponse.StatusCode);

        var loginMessage = new HttpRequestMessage(HttpMethod.Post,
            externalLoginUrl);
        loginMessage.Content = new FormUrlEncodedContent(new LoginModel
        {
            Code = fixture.PinCode, Token = fixture.GeneratedReviewLink.Token
        }.AsDictionary<string>());
        var loginResponse = await fixture.Client.SendAsync(loginMessage);
        var loginText = await loginResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Contains(fixture.ReviewedPageContent, loginText);
    }
}
