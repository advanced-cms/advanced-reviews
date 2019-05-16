using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web;

namespace AdvancedExternalReviews.DraftContentAreaPreview
{
    /// <summary>
    /// Modified version of ContentAreaLoader that loads
    /// draft content version in IsInExternalReviewContext context
    /// </summary>
    public class DraftContentAreaLoader : IContentAreaLoader
    {
        private readonly IContentAreaLoader _defaultContentAreaLoader;
        private readonly IContentLoader _contentLoader;
        private readonly LanguageResolver _languageResolver;
        private readonly IContentVersionRepository _contentVersionRepository;

        public DraftContentAreaLoader(IContentAreaLoader defaultContentAreaLoader, IContentLoader contentLoader, LanguageResolver languageResolver,
            IContentVersionRepository contentVersionRepository
        )
        {
            _defaultContentAreaLoader = defaultContentAreaLoader;
            _contentLoader = contentLoader;
            _languageResolver = languageResolver;
            _contentVersionRepository = contentVersionRepository;
        }

        public IContent Get(ContentAreaItem contentAreaItem)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentAreaLoader.Get(contentAreaItem);
            }

            // load common draft instead of published version
            var commonDraft = _contentVersionRepository.LoadCommonDraft(contentAreaItem.ContentLink,
                _languageResolver.GetPreferredCulture().Name);
            if (commonDraft != null)
            {
                var content = _contentLoader.Get<IContent>(commonDraft.ContentLink);
                if (content.IsPublished())
                {
                    // for published version return the original method result
                    var defaultContent = _defaultContentAreaLoader.Get(contentAreaItem);
                    return defaultContent;
                }

                contentAreaItem.ContentLink = commonDraft.ContentLink;

                return content;
            }

            return _defaultContentAreaLoader.Get(contentAreaItem);
        }

        public DisplayOption LoadDisplayOption(ContentAreaItem contentAreaItem)
        {
            return _defaultContentAreaLoader.LoadDisplayOption(contentAreaItem);
        }
    }
}
