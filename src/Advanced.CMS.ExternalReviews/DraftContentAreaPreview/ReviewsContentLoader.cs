﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview
{
    [ServiceConfiguration(typeof(ReviewsContentLoader))]
    public class ReviewsContentLoader
    {
        private readonly IContentLoader _contentLoader;
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IContentVersionRepository _contentVersionRepository;
        private readonly IContentProviderManager _contentProviderManager;
        private readonly IContentChildrenSorter _childrenSorter;
        private readonly ExternalReviewState _externalReviewState;
        private static readonly ILogger _log = LogManager.GetLogger(typeof(ReviewsContentLoader));

        public ReviewsContentLoader(IContentLoader contentLoader,
            ProjectContentResolver projectContentResolver,
            IContentVersionRepository contentVersionRepository,
            IContentProviderManager contentProviderManager,
            IContentChildrenSorter childrenSorter, ExternalReviewState externalReviewState)
        {
            _contentLoader = contentLoader;
            _projectContentResolver = projectContentResolver;
            _contentVersionRepository = contentVersionRepository;
            _contentProviderManager = contentProviderManager;
            _childrenSorter = childrenSorter;
            _externalReviewState = externalReviewState;
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink) where T : IContentData
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Fallback(new CultureInfo(_externalReviewState.PreferredLanguage)) };
            return GetChildrenWithReviews<T>(contentLink, loaderOptions);
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, CultureInfo language)
            where T : IContentData
        {
            return GetChildrenWithReviews<T>(contentLink,
                new LoaderOptions() { LanguageLoaderOption.Specific(language) }, -1, -1);
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, CultureInfo language,
            int startIndex, int maxRows) where T : IContentData
        {
            return GetChildrenWithReviews<T>(contentLink, language);
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, LoaderOptions loaderOptions)
            where T : IContentData
        {
            return GetChildrenWithReviews<T>(contentLink, loaderOptions, -1, -1);
        }

        public IEnumerable<T> GetChildrenWithReviews<T>(
            ContentReference contentLink, LoaderOptions loaderOptions, int startIndex, int maxRows)
            where T : IContentData
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                throw new ArgumentNullException(nameof(contentLink), "Parameter has no value set");
            }

            if (!_externalReviewState.IsInExternalReviewContext)
            {
                return _contentLoader.GetChildren<T>(contentLink);
            }

            var referenceWithoutVersion = contentLink.ToReferenceWithoutVersion();
            if (referenceWithoutVersion == ContentReference.WasteBasket)
            {
                return _contentLoader.GetChildren<T>(contentLink);
            }

            var provider = _contentProviderManager.ProviderMap.GetProvider(referenceWithoutVersion);

            var parentContent = _contentLoader.Get<IContent>(referenceWithoutVersion, loaderOptions);
            var languageID = parentContent is ILocalizable localizable ? localizable.Language.Name : null;
            var childrenReferences =
                provider.GetChildrenReferences<T>(referenceWithoutVersion, languageID, startIndex, maxRows);

            var result = new List<ContentReference>();
            foreach (var childReference in childrenReferences)
            {
                var referenceToLoad = LoadUnpublishedVersion(childReference.ContentLink);
                if (referenceToLoad == null)
                {
                    var publishedContentInTargetLanguage = _contentLoader.Get<IContent>(childReference.ContentLink, loaderOptions);
                    if (publishedContentInTargetLanguage != null)
                    {
                        result.Add(publishedContentInTargetLanguage.ContentLink);
                    }
                }
                else
                {
                    var content = _contentLoader.Get<T>(referenceToLoad);
                    if (!(content is IVersionable versionable))
                    {
                        result.Add(childReference.ContentLink);
                        continue;
                    }

                    if (HasExpired(versionable))
                    {
                        continue;
                    }

                    if ((content as IContent).IsPublished())
                    {
                        // for published version return the original method result
                        result.Add(childReference.ContentLink);
                        continue;
                    }

                    result.Add((content as IContent)?.ContentLink);
                }
            }

            var childrenWithReviews = result.Select(_contentLoader.Get<T>).ToList();


            if (childrenWithReviews.Count > 0)
            {
                var pageData = parentContent as PageData;
                if (pageData != null && pageData.ChildSortOrder == FilterSortOrder.Alphabetical &&
                    (startIndex == -1 && maxRows == -1 ||
                     childrenWithReviews.Count < maxRows && startIndex == 0))
                {
                    var collection = _childrenSorter.Sort((IEnumerable<IContent>) childrenWithReviews);
                    childrenWithReviews = collection.Cast<T>().ToList();
                }
            }

            return childrenWithReviews;
        }

        public ContentReference LoadUnpublishedVersion(ContentReference baseReference)
        {
            if (_externalReviewState.ProjectId.HasValue)
            {
                // load version from project
                return _projectContentResolver.GetProjectReference(baseReference, _externalReviewState.ProjectId.Value, _externalReviewState.PreferredLanguage);
            }

            // load common draft instead of published version
            ContentVersion loadCommonDraft;
            try
            {
                loadCommonDraft = _contentVersionRepository.LoadCommonDraft(baseReference, _externalReviewState.PreferredLanguage);
            }
            catch (ContentNotFoundException)
            {
                _log.Debug($"Advanced Reviews: Content {baseReference} not found for LoadUnpublishedVersion");
                loadCommonDraft = null;
            }

            if (loadCommonDraft == null)
            {
                // fallback to default implementation if there is no common draft in a given language
                return null;
            }

            return loadCommonDraft.ContentLink;
        }

        public bool HasExpired(IVersionable content)
        {
            return content.Status == VersionStatus.Published && content.StopPublish < DateTime.Now;
        }

        private LoaderOptions CreateDefaultListOption()
        {
            return new LoaderOptions { LanguageLoaderOption.Fallback(new CultureInfo(_externalReviewState.PreferredLanguage)) };
        }
    }
}
