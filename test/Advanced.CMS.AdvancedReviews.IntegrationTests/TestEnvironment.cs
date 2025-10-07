using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class TestEnvironment(StandardPage page, ExternalReviewLink externalReviewLink)
{
    public StandardPage Page { get; set; } = page;

    public ExternalReviewLink ExternalReviewLink { get; set; } = externalReviewLink;
}
