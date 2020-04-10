using System;
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
        private readonly ProjectContentResolver _projectContentResolver;

        public DraftContentAreaLoader(IContentAreaLoader defaultContentAreaLoader, IContentLoader contentLoader,
            LanguageResolver languageResolver,
            IContentVersionRepository contentVersionRepository, ProjectContentResolver projectContentResolver)
        {
            _defaultContentAreaLoader = defaultContentAreaLoader;
            _contentLoader = contentLoader;
            _languageResolver = languageResolver;
            _contentVersionRepository = contentVersionRepository;
            _projectContentResolver = projectContentResolver;
        }

        public IContent Get(ContentAreaItem contentAreaItem)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentAreaLoader.Get(contentAreaItem);
            }

            ContentReference referenceToLoad;
            if (ExternalReview.ProjectId.HasValue)
            {
                // load version from project
                referenceToLoad = _projectContentResolver.GetProjectReference(contentAreaItem.ContentLink,
                    ExternalReview.ProjectId.Value);
            }
            else
            {
                // load common draft instead of published version
                var loadCommonDraft = _contentVersionRepository.LoadCommonDraft(contentAreaItem.ContentLink,
                    _languageResolver.GetPreferredCulture().Name);
                if (loadCommonDraft == null)
                {
                    // fallback to default implementation if there is no common draft in a given language
                    return _defaultContentAreaLoader.Get(contentAreaItem);
                }
                referenceToLoad = loadCommonDraft.ContentLink;
            }

            if (referenceToLoad != null)
            {
                var content = _contentLoader.Get<IContent>(referenceToLoad);
                if (HasExpired(content as IVersionable))
                {
                    return null;
                }

                if (content.IsPublished())
                {
                    // for published version return the original method result
                    return _defaultContentAreaLoader.Get(contentAreaItem);
                }

                contentAreaItem.ContentLink = referenceToLoad;

                return content;
            }

            return _defaultContentAreaLoader.Get(contentAreaItem);
        }

        public DisplayOption LoadDisplayOption(ContentAreaItem contentAreaItem)
        {
            return _defaultContentAreaLoader.LoadDisplayOption(contentAreaItem);
        }

        private static bool HasExpired(IVersionable content)
        {
            return content.Status == VersionStatus.Published && content.StopPublish < DateTime.Now;
        }
    }
}
