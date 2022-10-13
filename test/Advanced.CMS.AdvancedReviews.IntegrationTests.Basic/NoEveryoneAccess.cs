using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

public class NoEveryoneAccess : IntegrationTestCollectionBaseClassFixture
{
    public NoEveryoneAccess(SiteFixture siteFixture) : base(siteFixture)
    {
    }

    [Fact]
    public async Task When_Creating_Token_For_Page_Without_Everyone_Access_It_Returns_200()
    {
        var testEnvironment = _testScenarioBuilder.Reset().WithoutEveryoneAccess().WithViewPin().Build();

        var message = new HttpRequestMessage(HttpMethod.Get, testEnvironment.ExternalReviewLink.LinkUrl);
        var response = await _siteFixture.Client.SendAsync(message);
        await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
