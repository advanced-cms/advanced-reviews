using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;

[CollectionDefinition(Name, DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<SiteFixture>
{
    public const string Name = "Advanced Reviews Test Collection";
}
