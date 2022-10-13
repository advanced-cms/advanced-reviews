using EPiServer.ServiceLocation;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Basic;

[CollectionDefinition(Name, DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<SiteFixture>
{
    public const string Name = "Advanced Reviews Test Collection";
}

[Collection(IntegrationTestCollection.Name)]
public class IntegrationTestCollectionBaseClassFixture : IClassFixture<CommonFixture>
{
    protected readonly SiteFixture _siteFixture;
    protected readonly TestScenarioBuilder _testScenarioBuilder;

    public IntegrationTestCollectionBaseClassFixture(SiteFixture siteFixture)
    {
        _siteFixture = siteFixture;
        _testScenarioBuilder = ServiceLocator.Current.GetInstance<TestScenarioBuilder>();
    }
}
