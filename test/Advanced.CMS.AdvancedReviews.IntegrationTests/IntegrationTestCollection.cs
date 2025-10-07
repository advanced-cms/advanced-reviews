using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using EPiServer.ServiceLocation;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<SiteFixture>
{
    public const string Name = "Advanced Reviews Test Collection";
}

[Collection(IntegrationTestCollection.Name)]
public class IntegrationTestCollectionBaseClassFixture(SiteFixture siteFixture) : IClassFixture<CommonFixture>
{
    protected readonly SiteFixture _siteFixture = siteFixture;
    protected readonly TestScenarioBuilderFactory _testScenarioBuilderFactory = ServiceLocator.Current.GetInstance<TestScenarioBuilderFactory>();
}
