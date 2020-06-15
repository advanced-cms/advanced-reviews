using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
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
        private readonly ReviewsContentLoader _reviewsContentLoader;

        public DraftContentAreaLoader(IContentAreaLoader defaultContentAreaLoader, IContentLoader contentLoader,
            ReviewsContentLoader reviewsContentLoader)
        {
            _defaultContentAreaLoader = defaultContentAreaLoader;
            _contentLoader = contentLoader;
            _reviewsContentLoader = reviewsContentLoader;
        }

        public IContent Get(ContentAreaItem contentAreaItem)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentAreaLoader.Get(contentAreaItem);
            }

            var referenceToLoad = _reviewsContentLoader.LoadUnpublishedVersion(contentAreaItem.ContentLink);
            if (referenceToLoad == null)
            {
                // fallback to default implementation if there is no common draft in a given language
                return _defaultContentAreaLoader.Get(contentAreaItem);
            }

            var content = _contentLoader.Get<IContent>(referenceToLoad);
            if (_reviewsContentLoader.HasExpired(content as IVersionable))
            {
                return null;
            }

            if (content.IsPublished())
            {
                // for published version return the original method result
                return _defaultContentAreaLoader.Get(contentAreaItem);
            }

            if (!contentAreaItem.IsReadOnly)
            {
                contentAreaItem.ContentLink = referenceToLoad;
            }

            return content;
        }

        public DisplayOption LoadDisplayOption(ContentAreaItem contentAreaItem)
        {
            return _defaultContentAreaLoader.LoadDisplayOption(contentAreaItem);
        }
    }
}
