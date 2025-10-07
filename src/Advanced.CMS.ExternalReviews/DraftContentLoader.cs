using System.Globalization;
using EPiServer.Cms.Shell;
using EPiServer.Core.Internal;
using EPiServer.Filters;

namespace Advanced.CMS.ExternalReviews;

internal class DraftContentLoader(
    IContentLoader defaultContentLoader,
    ExternalReviewState externalReviewState,
    IContentProviderManager contentProviderManager,
    IContentChildrenSorter childrenSorter)
    : ContentLoader
{
    public override T Get<T>(Guid contentGuid)
    {
        return defaultContentLoader.Get<T>(contentGuid);
    }

    public override T Get<T>(Guid contentGuid, LoaderOptions settings)
    {
        return defaultContentLoader.Get<T>(contentGuid, settings);
    }

    public override T Get<T>(Guid contentGuid, CultureInfo language)
    {
        return defaultContentLoader.Get<T>(contentGuid, language);
    }

    public override T Get<T>(ContentReference contentLink)
    {
        return defaultContentLoader.Get<T>(contentLink);
    }

    public override T Get<T>(ContentReference contentLink, CultureInfo language)
    {
        return defaultContentLoader.Get<T>(contentLink, language);
    }

    public override T Get<T>(ContentReference contentLink, LoaderOptions settings)
    {
        return defaultContentLoader.Get<T>(contentLink, settings);
    }

    public override IEnumerable<T> GetChildren<T>(ContentReference contentLink)
    {
        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink);
        }

        return GetChildrenWithReviews<T>(contentLink).ToList();
    }

    public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language)
    {
        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink, language);
        }

        return GetChildrenWithReviews<T>(contentLink, language);
    }

    public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings)
    {
        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink, settings);
        }

        return GetChildrenWithReviews<T>(contentLink, settings, -1, -1);
    }

    public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language,
        int startIndex, int maxRows)
    {
        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink, language, startIndex, maxRows);
        }

        return GetChildrenWithReviews<T>(contentLink, language, startIndex, maxRows);
    }

    public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings,
        int startIndex, int maxRows)
    {
        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink, settings, startIndex, maxRows);
        }

        return GetChildrenWithReviews<T>(contentLink, settings, startIndex, maxRows);
    }

    public override IEnumerable<ContentReference> GetDescendents(ContentReference contentLink)
    {
        return defaultContentLoader.GetDescendents(contentLink);
    }

    public override IEnumerable<IContent> GetAncestors(ContentReference contentLink)
    {
        return defaultContentLoader.GetAncestors(contentLink);
    }

    public override IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, CultureInfo language)
    {
        return defaultContentLoader.GetItems(contentLinks, language);
    }

    public override IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks,
        LoaderOptions settings)
    {
        return defaultContentLoader.GetItems(contentLinks, settings);
    }

    public override IContent GetBySegment(ContentReference parentLink, string urlSegment, CultureInfo language)
    {
        return defaultContentLoader.GetBySegment(parentLink, urlSegment, language);
    }

    public override IContent GetBySegment(ContentReference parentLink, string urlSegment, LoaderOptions settings)
    {
        return defaultContentLoader.GetBySegment(parentLink, urlSegment, settings);
    }

    public override bool TryGet<T>(ContentReference contentLink, out T content)
    {
        return defaultContentLoader.TryGet(contentLink, out content);
    }

    public override bool TryGet<T>(ContentReference contentLink, CultureInfo language, out T content)
    {
        return defaultContentLoader.TryGet(contentLink, language, out content);
    }

    public override bool TryGet<T>(ContentReference contentLink, LoaderOptions settings, out T content)
    {
        return defaultContentLoader.TryGet(contentLink, settings, out content);
    }

    public override bool TryGet<T>(Guid contentGuid, out T content)
    {
        return defaultContentLoader.TryGet(contentGuid, out content);
    }

    public override bool TryGet<T>(Guid contentGuid, CultureInfo language, out T content)
    {
        return defaultContentLoader.TryGet(contentGuid, language, out content);
    }

    public override bool TryGet<T>(Guid contentGuid, LoaderOptions loaderOptions, out T content)
    {
        return defaultContentLoader.TryGet(contentGuid, loaderOptions, out content);
    }

    private IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink) where T : IContentData
    {
        var loaderOptions = new LoaderOptions
            { LanguageLoaderOption.Fallback(new CultureInfo(externalReviewState.PreferredLanguage)) };
        return GetChildrenWithReviews<T>(contentLink, loaderOptions, -1, -1);
    }

    private IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, CultureInfo language)
        where T : IContentData
    {
        return GetChildrenWithReviews<T>(contentLink,
            new LoaderOptions { LanguageLoaderOption.Specific(language) }, -1, -1);
    }

    private IEnumerable<T> GetChildrenWithReviews<T>(ContentReference contentLink, CultureInfo language,
        int startIndex, int maxRows) where T : IContentData
    {
        return GetChildrenWithReviews<T>(contentLink, new LoaderOptions { LanguageLoaderOption.Specific(language) },
            startIndex, maxRows);
    }

    private IEnumerable<T> GetChildrenWithReviews<T>(
        ContentReference contentLink, LoaderOptions loaderOptions, int startIndex, int maxRows)
        where T : IContentData
    {
        if (ContentReference.IsNullOrEmpty(contentLink))
        {
            throw new ArgumentNullException(nameof(contentLink), "Parameter has no value set");
        }

        if (!externalReviewState.IsInExternalReviewContext)
        {
            return defaultContentLoader.GetChildren<T>(contentLink);
        }

        var referenceWithoutVersion = contentLink.ToReferenceWithoutVersion();
        if (referenceWithoutVersion == ContentReference.WasteBasket)
        {
            return defaultContentLoader.GetChildren<T>(contentLink);
        }

        var provider = contentProviderManager.ProviderMap.GetProvider(referenceWithoutVersion);

        var parentContent = defaultContentLoader.Get<IContent>(referenceWithoutVersion, loaderOptions);
        var languageID = parentContent is ILocalizable localizable ? localizable.Language.Name : null;
        var childrenReferences =
            provider.GetChildrenReferences<T>(referenceWithoutVersion, languageID, startIndex, maxRows);

        var result = new List<ContentReference>();
        foreach (var childReference in childrenReferences)
        {
            var referenceToLoad = childReference.ContentLink.LoadUnpublishedVersion();
            if (referenceToLoad == null)
            {
                var publishedContentInTargetLanguage =
                    defaultContentLoader.Get<IContent>(childReference.ContentLink, loaderOptions);
                if (publishedContentInTargetLanguage != null)
                {
                    result.Add(publishedContentInTargetLanguage.ContentLink);
                }
            }
            else
            {
                var content = defaultContentLoader.Get<T>(referenceToLoad);
                if (!(content is IVersionable versionable))
                {
                    result.Add(childReference.ContentLink);
                    continue;
                }

                if (versionable.HasExpired())
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

        var childrenWithReviews = result.Select(defaultContentLoader.Get<T>).ToList();


        if (childrenWithReviews.Count > 0)
        {
            var pageData = parentContent as PageData;
            if (pageData != null && pageData.ChildSortOrder == FilterSortOrder.Alphabetical &&
                (startIndex == -1 && maxRows == -1 ||
                 childrenWithReviews.Count < maxRows && startIndex == 0))
            {
                var collection = childrenSorter.Sort((IEnumerable<IContent>)childrenWithReviews);
                childrenWithReviews = collection.Cast<T>().ToList();
            }
        }

        return childrenWithReviews;
    }
}
