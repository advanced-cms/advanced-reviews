using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class TestEnvironment
{
    public TestEnvironment(StandardPage page, ExternalReviewLink externalReviewLink)
    {
        Page = page;
        ExternalReviewLink = externalReviewLink;
    }

    public StandardPage Page { get; set; }

    public ExternalReviewLink ExternalReviewLink { get; set; }
}
