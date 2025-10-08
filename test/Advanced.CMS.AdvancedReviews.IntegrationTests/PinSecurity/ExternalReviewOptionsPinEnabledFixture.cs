using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity;

public class ExternalReviewOptionsPinEnabledFixture(SiteFixture siteFixture) : OptionsOverrideFixture(siteFixture)
{
    protected override void ApplyOverride(ExternalReviewOptions options)
    {
        options.PinCodeSecurity.Enabled = true;
    }
}
