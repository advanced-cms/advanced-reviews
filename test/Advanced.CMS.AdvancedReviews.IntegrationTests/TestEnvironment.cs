using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Core;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class TestEnvironment
{
    public TestEnvironment(PageData page, ExternalReviewLink externalReviewLink)
    {
        Page = page;
        ExternalReviewLink = externalReviewLink;
    }

    public PageData Page { get; set; }

    public ExternalReviewLink ExternalReviewLink { get; set; }
}
