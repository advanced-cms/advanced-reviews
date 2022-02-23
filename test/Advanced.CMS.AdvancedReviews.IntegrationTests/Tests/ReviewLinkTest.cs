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
    public class SinglePageTests : IntegrationTestCollectionBaseClassFixture
    {
        private readonly IContentRepository _contentRepository;

        public SinglePageTests(SiteFixture siteFixture) : base(siteFixture)
        {
            _contentRepository = siteFixture.Services.GetService<IContentRepository>();
        }

        [Fact]
        public async Task When_DraftPage_Created_Link_Is_Active_And_After_Expiration_It_Returns_404()
        {
            var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
            page.PageName = "test page";
            var contentLink = _contentRepository.Save(page, AccessLevel.NoAccess);
            var reviewLinksRepository =
                _siteFixture.Services.GetRequiredService<IExternalReviewLinksRepository>();
            reviewLinksRepository.GetLinksForContent(contentLink, null);
            var link = reviewLinksRepository.AddLink(contentLink, false, TimeSpan.FromDays(1), null);
            var message = new HttpRequestMessage(HttpMethod.Get, link.LinkUrl);
            var response = await _siteFixture.Client.SendAsync(message);
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedLink = reviewLinksRepository.UpdateLink(link.Token,
                DateTime.Now.Subtract(TimeSpan.FromSeconds(1)),
                null, null,
                null);

            var message2 = new HttpRequestMessage(HttpMethod.Get, updatedLink.LinkUrl);
            var response2 = await _siteFixture.Client.SendAsync(message2);
            var text2 = await response2.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
        }
    }
}
