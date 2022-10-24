using Advanced.CMS.ExternalReviews;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity;

public class SiteFixtureWithPinSecurity : SiteFixtureBase
{
    public SiteFixtureWithPinSecurity() : base(OptionsCallback)
    {
    }

    private static void OptionsCallback(IServiceCollection services)
    {
        services.Configure<ExternalReviewOptions>(options =>
        {
            options.PinCodeSecurity.Enabled = true;
        });
    }
}
