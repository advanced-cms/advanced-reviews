using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using Microsoft.Extensions.DependencyInjection;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic
{
    public class SinglePageTests : IntegrationTestCollectionBaseClassFixture
    {
        private readonly IContentRepository _contentRepository;
        private readonly IContentSecurityRepository _contentSecurityRepository;

        public SinglePageTests(SiteFixture siteFixture) : base(siteFixture)
        {
            _contentRepository = siteFixture.Services.GetService<IContentRepository>();
            _contentSecurityRepository = siteFixture.Services.GetService<IContentSecurityRepository>();
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
            await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedLink = reviewLinksRepository.UpdateLink(link.Token,
                DateTime.Now.Subtract(TimeSpan.FromSeconds(1)),
                null, null,
                null);

            var messageAfterTokenExpiration = new HttpRequestMessage(HttpMethod.Get, updatedLink.LinkUrl);
            var responseAfterTokenExpiration = await _siteFixture.Client.SendAsync(messageAfterTokenExpiration);
            await responseAfterTokenExpiration.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, responseAfterTokenExpiration.StatusCode);
        }

        [Fact]
        public async Task When_Creating_Token_For_Page_Without_Everyone_Access_It_Returns_200()
        {
            var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
            page.PageName = "test page";
            var contentLink = _contentRepository.Save(page, AccessLevel.NoAccess);
            var accessControlList = new AccessControlList
            {
                new AccessControlEntry(EPiServer.Authorization.Roles.CmsAdmins, AccessLevel.Administer)
            };
            accessControlList.IsInherited = false;
            page.SaveSecurityInfo(this._contentSecurityRepository, accessControlList, SecuritySaveType.Replace);

            var reviewLinksRepository =
                _siteFixture.Services.GetRequiredService<IExternalReviewLinksRepository>();
            reviewLinksRepository.GetLinksForContent(contentLink, null);
            var link = reviewLinksRepository.AddLink(contentLink, false, TimeSpan.FromDays(1), null);
            var message = new HttpRequestMessage(HttpMethod.Get, link.LinkUrl);
            var response = await _siteFixture.Client.SendAsync(message);
            await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
