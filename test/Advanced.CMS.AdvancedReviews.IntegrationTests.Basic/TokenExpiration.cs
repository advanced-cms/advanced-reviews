using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

public class TokenExpiration : IntegrationTestCollectionBaseClassFixture
{
    public TokenExpiration(SiteFixture siteFixture) : base(siteFixture)
    {
    }

    [Fact]
    public async Task When_DraftPage_Created_Link_Is_Expired_It_Returns_404()
    {
        var testEnvironment = _testScenarioBuilderFactory.GetBuilder().Init().WithViewPin().PinExpired().Build();

        var messageAfterTokenExpiration = new HttpRequestMessage(HttpMethod.Get, testEnvironment.ExternalReviewLink.LinkUrl);
        var responseAfterTokenExpiration = await _siteFixture.Client.SendAsync(messageAfterTokenExpiration);
        await responseAfterTokenExpiration.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, responseAfterTokenExpiration.StatusCode);
    }
}
