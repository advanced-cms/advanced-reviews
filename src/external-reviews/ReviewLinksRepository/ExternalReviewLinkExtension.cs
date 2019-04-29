using System;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    public static class ExternalReviewLinkExtension
    {
        public static bool IsExpired(this ExternalReviewLink externalReviewLink)
        {
            return externalReviewLink.ValidTo < DateTime.Now;
        }
    }
}
