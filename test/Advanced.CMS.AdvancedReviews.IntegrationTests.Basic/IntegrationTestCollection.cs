using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests
{
    [CollectionDefinition(Name, DisableParallelization = true)]
    public class IntegrationTestCollection : ICollectionFixture<SiteFixture>
    {
        public const string Name = "Advanced Reviews Test Collection";
    }

    [Collection(IntegrationTestCollection.Name)]
    public class IntegrationTestCollectionBaseClassFixture : IClassFixture<CommonFixture>
    {
        protected readonly SiteFixture _siteFixture;

        public IntegrationTestCollectionBaseClassFixture(SiteFixture siteFixture)
        {
            _siteFixture = siteFixture;
        }
    }
}
