using EPiServer.Core;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview;

internal class PublishedStateAssessorDecorator : IPublishedStateAssessor
{
    private readonly IPublishedStateAssessor _defaultService;
    private readonly ExternalReviewState _externalReviewState;

    public PublishedStateAssessorDecorator(IPublishedStateAssessor defaultService, ExternalReviewState externalReviewState)
    {
        _defaultService = defaultService;
        _externalReviewState = externalReviewState;
    }

    public bool IsPublished(IContent content, PublishedStateCondition condition)
    {
        if (_externalReviewState.IsInExternalReviewContext)
        {
            if (content is PageData)
            {
                return true;
            }

            if (_externalReviewState.CustomLoaded.Contains(content.ContentLink.ToString()))
            {
                var cachedContent = _externalReviewState.GetCachedContent(content.ContentLink);
                if (cachedContent != null)
                {
                    return true;
                }
            }
        }

        return _defaultService.IsPublished(content, condition);
    }
}
