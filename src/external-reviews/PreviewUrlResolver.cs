using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews
{
    public class PreviewUrlResolver : UrlResolver
    {
        private const string PreviewGenerated = "preview_generated";
        private readonly UrlResolver _defaultUrlResolver;
        private readonly IContentLoader _contentLoader;
        private readonly Injected<ExternalReviewOptions> _externalReviewOptions;
        private readonly IPermanentLinkMapper _permanentLinkMapper;
        private readonly IContentProviderManager _providerManager;

        public PreviewUrlResolver(UrlResolver defaultUrlResolver, IContentLoader contentLoader,
            IPermanentLinkMapper permanentLinkMapper, IContentProviderManager providerManager)
        {
            _defaultUrlResolver = defaultUrlResolver;
            _contentLoader = contentLoader;
            _permanentLinkMapper = permanentLinkMapper;
            _providerManager = providerManager;
        }

        public override IContent Route(UrlBuilder urlBuilder, ContextMode contextMode)
        {
            return _defaultUrlResolver.Route(urlBuilder, contextMode);
        }

        public static bool IsGeneratedForProjectPreview(NameValueCollection queryString)
        {
            return queryString[PreviewGenerated] != null;
        }

        public override VirtualPathData GetVirtualPath(ContentReference contentLink, string language,
            VirtualPathArguments virtualPathArguments)
        {
            var virtualPathData = _defaultUrlResolver.GetVirtualPath(contentLink, language, virtualPathArguments);
            if (!ExternalReview.IsInProjectReviewContext || !ExternalReview.IsInExternalReviewContext || virtualPathData == null)
            {
                return virtualPathData;
            }

            var content = _contentLoader.Get<IContent>(contentLink);
            if (content is PageData data)
            {
                var virtualPath = GetAccessibleVirtualPath(virtualPathData, data, language);
                virtualPathData.VirtualPath = AppendGeneratedPostfix(virtualPath);
            }

            return virtualPathData;
        }

        /// <summary>
        /// Get a virtual path that can be accessed. If URL segment has been changed we need
        /// to return the original (published) version's URL segment for the routing to work.
        /// </summary>
        private string GetAccessibleVirtualPath(VirtualPathData virtualPathData, PageData data, string language)
        {
            var virtualPath = virtualPathData.VirtualPath;
            var provider = this._providerManager.ProviderMap.GetProvider(data.ContentLink.ProviderName);
            var masterContent = (PageData)provider.GetScatteredContents(new[] {data.ContentLink.ToReferenceWithoutVersion()},
                new LanguageSelector(language ?? data.LanguageBranch())).FirstOrDefault();
            if (masterContent != null)
            {
                var urlSegment = data.URLSegment;
                var masterContentUrlSegment = masterContent.URLSegment;
                if (masterContentUrlSegment != urlSegment)
                {
                    var count = Regex.Matches(virtualPath, Regex.Escape(urlSegment)).Count;
                    // If there are multiple occurrences we should just skip the replace to avoid problems
                    if (count == 1)
                    {
                        virtualPath = virtualPath.Replace(urlSegment, masterContentUrlSegment);
                    }
                }
            }

            return virtualPath;
        }

        public override string GetUrl(UrlBuilder urlBuilderWithInternalUrl, VirtualPathArguments arguments)
        {
            var url = _defaultUrlResolver.GetUrl(urlBuilderWithInternalUrl, arguments);
            if (!ExternalReview.IsInProjectReviewContext || !ExternalReview.IsInExternalReviewContext || url == null)
            {
                return url;
            }

            var linkMap = _permanentLinkMapper.Find(urlBuilderWithInternalUrl);

            if (linkMap != null)
            {
                var content = _contentLoader.Get<IContent>(linkMap.ContentReference);
                if (content is PageData)
                {
                    return AppendGeneratedPostfix(url);
                }
            }

            return url;
        }

        public override bool TryToPermanent(string url, out string permanentUrl)
        {
            return _defaultUrlResolver.TryToPermanent(url, out permanentUrl);
        }

        public override VirtualPathData GetVirtualPathForNonContent(object partialRoutedObject, string language,
            VirtualPathArguments virtualPathArguments)
        {
            return _defaultUrlResolver.GetVirtualPathForNonContent(partialRoutedObject, language, virtualPathArguments);
        }

        private string AppendGeneratedPostfix(string url)
        {
            var urlBuilder = new UrlBuilder(UrlPath.Combine(url, _externalReviewOptions.Service.ContentPreviewUrl, ExternalReview.Token));
            urlBuilder.QueryCollection.Add(PreviewGenerated, bool.TrueString);
            return urlBuilder.ToString();
        }
    }
}
