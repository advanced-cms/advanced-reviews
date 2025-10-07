using System.Net;
using Advanced.CMS.ExternalReviews;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.Extensions.DependencyInjection;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity;

[Collection(IntegrationTestCollectionWithPinSecurity.Name)]
public class SinglePageWithPinSecurityTests(SiteFixtureWithPinSecurity siteFixture) : IClassFixture<CommonFixture>
{
    private readonly IContentRepository _contentRepository = siteFixture.Services.GetService<IContentRepository>();
    private readonly ExternalReviewOptions _externalReviewOptions = siteFixture.Services.GetService<ExternalReviewOptions>();
    private readonly ServiceAccessor<SiteDefinition> _currentSiteDefinition = siteFixture.Services.GetService<ServiceAccessor<SiteDefinition>>();

    [Fact]
    public async Task When_PinSecurity_Enabled_Login_Screen_Is_Shown()
    {
        var siteDefinition = _currentSiteDefinition();
        const string reviewedPageContent = "test page";

        var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
        page.PageName = reviewedPageContent;
        var contentLink = _contentRepository.Save(page, AccessLevel.NoAccess);
        var reviewLinksRepository =
            siteFixture.Services.GetRequiredService<IExternalReviewLinksRepository>();
        reviewLinksRepository.GetLinksForContent(contentLink, null);
        var link = reviewLinksRepository.AddLink(contentLink, false, TimeSpan.FromDays(1), null);
        const string pinCode = "123";
        reviewLinksRepository.UpdateLink(link.Token, DateTime.Now.AddDays(1), pinCode, null, null);
        var message = new HttpRequestMessage(HttpMethod.Get, link.LinkUrl);
        var response = await siteFixture.Client.SendAsync(message);
        var text = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Go", text);

        var externalLoginUrl = new Uri(siteDefinition.SiteUrl, _externalReviewOptions.PinCodeSecurity.ExternalReviewLoginUrl);
        var failedLoginMessage = new HttpRequestMessage(HttpMethod.Post,
            externalLoginUrl);
        failedLoginMessage.Content = new FormUrlEncodedContent(new LoginModel
        {
            Code = "111111111111111", Token = link.Token
        }.AsDictionary<string>());
        var failedLoginResponse = await siteFixture.Client.SendAsync(failedLoginMessage);
        Assert.Equal(HttpStatusCode.NotFound, failedLoginResponse.StatusCode);

        var loginMessage = new HttpRequestMessage(HttpMethod.Post,
            externalLoginUrl);
        loginMessage.Content = new FormUrlEncodedContent(new LoginModel
        {
            Code = pinCode, Token = link.Token
        }.AsDictionary<string>());
        var loginResponse = await siteFixture.Client.SendAsync(loginMessage);
        var loginText = await loginResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Contains(reviewedPageContent, loginText);
    }
}
