using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity;

[CollectionDefinition(Name, DisableParallelization = true)]
public class IntegrationTestCollectionWithPinSecurity : ICollectionFixture<SiteFixtureWithPinSecurity>
{
    public const string Name = "Advanced Reviews Test Collection With Pin Security";
}
