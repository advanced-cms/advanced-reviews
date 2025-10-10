using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Children_Loader;

public class WithGetChildrenOverride(SiteFixture siteFixture) : OptionsOverrideFixture(siteFixture)
{
    protected override void ApplyOverride(ExternalReviewOptions options)
    {
        options.InterceptGetChildren = true;
    }
}
