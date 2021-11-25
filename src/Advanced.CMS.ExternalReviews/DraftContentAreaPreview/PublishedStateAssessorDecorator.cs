using EPiServer.Core;
using EPiServer.Globalization;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview
{
    public class PublishedStateAssessorDecorator : IPublishedStateAssessor
    {
        private readonly IPublishedStateAssessor _defaultService;
        private readonly LanguageResolver _languageResolver;
        private readonly ExternalReviewState _externalReviewState;

        public PublishedStateAssessorDecorator(IPublishedStateAssessor defaultService,
            LanguageResolver languageResolver, ExternalReviewState externalReviewState)
        {
            _defaultService = defaultService;
            _languageResolver = languageResolver;
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
                    var cachedContent =
                        _externalReviewState.GetCachedContent(_languageResolver.GetPreferredCulture(), content.ContentLink);
                    if (cachedContent != null)
                    {
                        return true;
                    }
                }
            }

            return _defaultService.IsPublished(content, condition);
        }
    }
}
