using Advanced.CMS.ExternalReviews;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity
{
    public class SiteFixtureWithPinSecurity : SiteFixtureBase
    {
        public SiteFixtureWithPinSecurity() : base(OptionsCallback)
        {
        }

        private static void OptionsCallback(ExternalReviewOptions options)
        {
            options.PinCodeSecurity.Enabled = true;
        }
    }
}
