using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Filters;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
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

                if (!contentAreaItem.IsReadOnly)
                {
                    contentAreaItem.ContentLink = referenceToLoad;
                }

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

    [ServiceConfiguration(typeof(ReviewsContentLoader))]
    public class ReviewsContentLoader
    {
        private readonly IContentLoader _contentLoader;
        private readonly IContentLanguageAccessor _languageAccessor;
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IContentVersionRepository _contentVersionRepository;
        private readonly LanguageResolver _languageResolver;
        private readonly IContentProviderManager _contentProviderManager;
        private readonly IContentChildrenSorter _childrenSorter;

        public ReviewsContentLoader(IContentLoader contentLoader, IContentLanguageAccessor languageAccessor,
            ProjectContentResolver projectContentResolver,
            IContentVersionRepository contentVersionRepository,
            LanguageResolver languageResolver,
            IContentProviderManager contentProviderManager,
            IContentChildrenSorter childrenSorter)
        {
            _contentLoader = contentLoader;
            _languageAccessor = languageAccessor;
            _projectContentResolver = projectContentResolver;
            _contentVersionRepository = contentVersionRepository;
            _languageResolver = languageResolver;
            _contentProviderManager = contentProviderManager;
            _childrenSorter = childrenSorter;
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink) where T : IContent
        {
            return this.GetChildrenWithReviews<T>(contentLink, this.CreateDefaultListOption());
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, LoaderOptions loaderOptions) where T : IContent
        {
            return this.GetChildrenWithReviews<T>(contentLink, loaderOptions, -1, -1);
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(
            ContentReference contentLink, LoaderOptions loaderOptions, int startIndex, int maxRows) where T : IContent
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                throw new ArgumentNullException(nameof(contentLink), "Parameter has no value set");
            }

            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _contentLoader.GetChildren<T>(contentLink);
            }

            ContentReference referenceWithoutVersion = contentLink.ToReferenceWithoutVersion();
            if (referenceWithoutVersion == (ContentReference)ContentReference.WasteBasket)
            {
                return _contentLoader.GetChildren<T>(contentLink);
            }

            var provider = _contentProviderManager.ProviderMap.GetProvider(referenceWithoutVersion);

            var parentContent = _contentLoader.Get<IContent>(referenceWithoutVersion, loaderOptions);
            var localizable = parentContent as ILocalizable;
            var languageID = localizable != null ? localizable.Language.Name : (string)null;
            var childrenReferences =
                provider.GetChildrenReferences<T>(referenceWithoutVersion, languageID, startIndex, maxRows);

            var result = new List<ContentReference>();
            foreach (var childReference in childrenReferences)
            {
                ContentReference referenceToLoad;
                if (ExternalReview.ProjectId.HasValue)
                {
                    // load version from project
                    referenceToLoad = _projectContentResolver.GetProjectReference(childReference.ContentLink,
                        ExternalReview.ProjectId.Value);
                }
                else
                {
                    // load common draft instead of published version
                    var loadCommonDraft = _contentVersionRepository.LoadCommonDraft(childReference.ContentLink,
                        _languageResolver.GetPreferredCulture().Name);
                    if (loadCommonDraft == null)
                    {
                        // fallback to default implementation if there is no common draft in a given language
                        result.Add(childReference.ContentLink);
                        continue;
                    }

                    referenceToLoad = loadCommonDraft.ContentLink;
                }

                if (referenceToLoad != null)
                {
                    var content = _contentLoader.Get<T>(referenceToLoad);
                    if (HasExpired(content as IVersionable))
                    {
                        continue;
                    }

                    if (content.IsPublished())
                    {
                        // for published version return the original method result
                        result.Add(childReference.ContentLink);
                        continue;
                    }

                    result.Add(content.ContentLink);
                }
            }

            var childrenWithReviews = result.Select(_contentLoader.Get<T>).ToList();


            if (childrenWithReviews.Count > 0)
            {
                var pageData = parentContent as PageData;
                if (pageData != null && pageData.ChildSortOrder == FilterSortOrder.Alphabetical && (startIndex == -1 && maxRows == -1 ||
                                                                                                    childrenWithReviews.Count < maxRows && startIndex == 0))
                {
                    var collection = this._childrenSorter.Sort((IEnumerable<IContent>) childrenWithReviews);
                    childrenWithReviews = collection.Cast<T>().ToList();
                }
            }

            return childrenWithReviews;
        }

        private bool HasExpired(IVersionable content)
        {
            return content.Status == VersionStatus.Published && content.StopPublish < DateTime.Now;
        }

        private LoaderOptions CreateDefaultListOption()
        {
            LoaderOptions loaderOptions = new LoaderOptions();
            loaderOptions.Add<LanguageLoaderOption>(LanguageLoaderOption.Fallback(this._languageAccessor.Language));
            return loaderOptions;
        }
    }
}
