using EPiServer.Core;
using EPiServer.Globalization;

namespace AdvancedExternalReviews.DraftContentAreaPreview
{
    public class PublishedStateAssessorDecorator : IPublishedStateAssessor
    {
        private readonly IPublishedStateAssessor _defaultService;
        private readonly LanguageResolver _languageResolver;

        public PublishedStateAssessorDecorator(IPublishedStateAssessor defaultService, LanguageResolver languageResolver)
        {
            _defaultService = defaultService;
            _languageResolver = languageResolver;
        }

        public bool IsPublished(IContent content, PublishedStateCondition condition)
        {
            if (ExternalReview.IsInExternalReviewContext)
            {
                var cachedContent = ExternalReview.GetCachedContent(_languageResolver.GetPreferredCulture(), content.ContentLink);
                if (cachedContent != null)
                {
                    return true;
                }
            }

            return _defaultService.IsPublished(content, condition);
        }
    }
}
