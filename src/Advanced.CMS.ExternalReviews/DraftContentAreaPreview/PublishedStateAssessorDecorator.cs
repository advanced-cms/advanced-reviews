namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview;

internal class PublishedStateAssessorDecorator(
    IPublishedStateAssessor defaultService,
    ExternalReviewState externalReviewState)
    : IPublishedStateAssessor
{
    public bool IsPublished(IContent content, PublishedStateCondition condition)
    {
        if (externalReviewState.IsInExternalReviewContext)
        {
            if (content is PageData)
            {
                return true;
            }

            if (externalReviewState.CustomLoaded.Contains(content.ContentLink.ToString()))
            {
                var cachedContent = externalReviewState.GetCachedContent(content.ContentLink);
                if (cachedContent != null)
                {
                    return true;
                }
            }
        }

        return defaultService.IsPublished(content, condition);
    }
}
