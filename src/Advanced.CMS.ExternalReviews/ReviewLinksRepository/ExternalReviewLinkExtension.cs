namespace Advanced.CMS.ExternalReviews.ReviewLinksRepository;

public static class ExternalReviewLinkExtension
{
    public static bool IsExpired(this ExternalReviewLink externalReviewLink)
    {
        return externalReviewLink.ValidTo < DateTime.Now;
    }

    public static bool IsEditableLink(this ExternalReviewLink externalReviewLink)
    {
        return externalReviewLink != null &&
               !externalReviewLink.IsExpired() &&
               externalReviewLink.IsEditable;
    }

    public static bool IsPreviewableLink(this ExternalReviewLink externalReviewLink)
    {
        return externalReviewLink != null &&
               !externalReviewLink.IsExpired() &&
               !externalReviewLink.IsEditable;
    }
}
