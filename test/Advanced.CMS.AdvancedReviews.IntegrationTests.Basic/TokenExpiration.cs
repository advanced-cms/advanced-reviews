using System.Net;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

public class TokenExpiration(SiteFixture siteFixture) : IntegrationTestCollectionBaseClassFixture(siteFixture)
{
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
