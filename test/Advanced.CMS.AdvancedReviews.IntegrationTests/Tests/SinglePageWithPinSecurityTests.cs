using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using Microsoft.Extensions.DependencyInjection;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Tests
{
    [Collection(IntegrationTestCollectionWithPinSecurity.Name)]
    public class SinglePageWithPinSecurityTests : IClassFixture<CommonFixture>
    {
        private readonly SiteFixtureWithPinSecurity _siteFixture;
        private readonly IContentRepository _contentRepository;

        public SinglePageWithPinSecurityTests(SiteFixtureWithPinSecurity siteFixture)
        {
            _siteFixture = siteFixture;
            _contentRepository = siteFixture.Services.GetService<IContentRepository>();
        }

        [Fact]
        public async Task When_PinSecurity_Enabled_Login_Screen_Is_Shown()
        {
            var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
            page.PageName = "test page";
            var contentLink = _contentRepository.Save(page, AccessLevel.NoAccess);
            var reviewLinksRepository =
                _siteFixture.Services.GetRequiredService<IExternalReviewLinksRepository>();
            reviewLinksRepository.GetLinksForContent(contentLink, null);
            var link = reviewLinksRepository.AddLink(contentLink, false, TimeSpan.FromDays(1), null);
            reviewLinksRepository.UpdateLink(link.Token, DateTime.Now.AddDays(1), "123", null, null);
            var message = new HttpRequestMessage(HttpMethod.Get, link.LinkUrl);
            var response = await _siteFixture.Client.SendAsync(message);
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Go", text);

            //TODO: test failing login scenario
            //TODO: test success login scenario
        }
    }
}
