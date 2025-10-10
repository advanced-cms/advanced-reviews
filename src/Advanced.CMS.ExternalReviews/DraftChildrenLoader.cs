using System.Globalization;
using EPiServer.Cms.Shell;
using EPiServer.Core.Internal;
using EPiServer.Filters;

namespace Advanced.CMS.ExternalReviews;

public class DraftChildrenLoader(
    IContentLoader contentLoader,
    IContentProviderManager contentProviderManager,
    IContentChildrenSorter childrenSorter)
{
    public IEnumerable<IContent> GetChildrenWithReviews(ContentReference contentLink, CultureInfo language)
    {
        if (ContentReference.IsNullOrEmpty(contentLink))
        {
            return [];
        }

        var referenceWithoutVersion = contentLink.ToReferenceWithoutVersion();
        if (referenceWithoutVersion == ContentReference.WasteBasket)
        {
            return contentLoader.GetChildren<IContent>(contentLink, language);
        }

        var provider = contentProviderManager.ProviderMap.GetProvider(referenceWithoutVersion);
        var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(language) };

        var parentContent = contentLoader.Get<IContent>(referenceWithoutVersion, loaderOptions);
        var languageID = parentContent is ILocalizable localizable ? localizable.Language.Name : null;
        var childrenReferences = provider.GetChildrenReferences<IContent>(referenceWithoutVersion, languageID, -1, -1);

        var result = new List<ContentReference>();
        foreach (var childReference in childrenReferences)
        {
            var referenceToLoad = childReference.ContentLink.LoadUnpublishedVersion();
            if (referenceToLoad == null)
            {
                var publishedContentInTargetLanguage =
                    contentLoader.Get<IContent>(childReference.ContentLink, loaderOptions);
                if (publishedContentInTargetLanguage != null)
                {
                    result.Add(publishedContentInTargetLanguage.ContentLink);
                }
            }
            else
            {
                var content = contentLoader.Get<IContent>(referenceToLoad);
                if (!(content is IVersionable versionable))
                {
                    result.Add(childReference.ContentLink);
                    continue;
                }

                if (versionable.HasExpired())
                {
                    continue;
                }

                if (content.IsPublished())
                {
                    result.Add(childReference.ContentLink);
                    continue;
                }

                result.Add(content.ContentLink);
            }
        }

        var childrenWithReviews = result.Select(contentLoader.Get<IContent>).ToList();

        if (childrenWithReviews.Count > 0)
        {
            var pageData = parentContent as PageData;
            if (pageData != null && pageData.ChildSortOrder == FilterSortOrder.Alphabetical)
            {
                var collection = childrenSorter.Sort(childrenWithReviews);
                childrenWithReviews = collection.ToList();
            }
        }

        return childrenWithReviews;
    }
}
