using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class Given_Block_With_CommonDraft_And_ProjectDraft : IntegrationTestCollectionBaseClassFixture
{
    protected TestEnvironment testEnvironment;

    public Given_Block_With_CommonDraft_And_ProjectDraft(SiteFixture siteFixture) : base(siteFixture)
    {
        testEnvironment = _testScenarioBuilderFactory.GetBuilder().Init().AddBlock().SwitchToProject().UpdateBlock()
            .Build();
    }

    public class When_Adding_Link_In_ProjectMode : Given_Block_With_CommonDraft_And_ProjectDraft
    {
        public When_Adding_Link_In_ProjectMode(SiteFixture siteFixture) : base(siteFixture)
        {
            testEnvironment = _testScenarioBuilderFactory.GetBuilder(testEnvironment)
                .WithViewPin().Build();
        }

        [Fact]
        public async Task Should_Load_ProjectDraft()
        {
            var message = new HttpRequestMessage(HttpMethod.Get, testEnvironment.ExternalReviewLink.LinkUrl);
            var response = await _siteFixture.Client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Contains(TestScenarioBuilder.ProjectUpdatedString, responseText);
        }
    }

    public class When_Adding_Link_In_CommonDraftsMode : Given_Block_With_CommonDraft_And_ProjectDraft
    {
        public When_Adding_Link_In_CommonDraftsMode(SiteFixture siteFixture) : base(siteFixture)
        {
            testEnvironment = _testScenarioBuilderFactory.GetBuilder(testEnvironment).SwitchToCommonDrafts()
                .WithViewPin().Build();
        }

        [Fact]
        public async Task Should_Load_Common_Draft()
        {
            var message = new HttpRequestMessage(HttpMethod.Get, testEnvironment.ExternalReviewLink.LinkUrl);
            var response = await _siteFixture.Client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseText = await response.Content.ReadAsStringAsync();
            Assert.Contains(TestScenarioBuilder.OriginalBlockContent, responseText);
        }
    }
}
