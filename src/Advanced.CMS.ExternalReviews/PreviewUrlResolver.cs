using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Advanced.CMS.ExternalReviews
{
    public class PreviewUrlResolver : IUrlResolver
    {
        private const string PreviewGenerated = "preview_generated";
        private readonly IUrlResolver _defaultUrlResolver;
        private readonly IContentLoader _contentLoader;
        private readonly Injected<ExternalReviewOptions> _externalReviewOptions;
        private readonly IPermanentLinkMapper _permanentLinkMapper;
        private readonly IContentProviderManager _providerManager;
        private readonly ExternalReviewState _externalReviewState;
        private readonly ExternalReviewUrlGenerator _externalReviewUrlGenerator;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;

        public PreviewUrlResolver(IUrlResolver defaultUrlResolver, IContentLoader contentLoader,
            IPermanentLinkMapper permanentLinkMapper, IContentProviderManager providerManager,
            ExternalReviewState externalReviewState, ExternalReviewUrlGenerator externalReviewUrlGenerator,
            ISiteDefinitionResolver siteDefinitionResolver)
        {
            _defaultUrlResolver = defaultUrlResolver;
            _contentLoader = contentLoader;
            _permanentLinkMapper = permanentLinkMapper;
            _providerManager = providerManager;
            _externalReviewState = externalReviewState;
            _externalReviewUrlGenerator = externalReviewUrlGenerator;
            _siteDefinitionResolver = siteDefinitionResolver;
        }

        public static bool IsGeneratedForProjectPreview(NameValueCollection queryString)
        {
            return queryString[PreviewGenerated] != null;
        }

        public string GetVirtualPath(ContentReference contentLink, string language,
            UrlResolverArguments virtualPathArguments)
        {
            var virtualPathData = _defaultUrlResolver.GetUrl(contentLink, language, virtualPathArguments);
            if (!_externalReviewState.IsInExternalReviewContext || virtualPathData == null)
            {
                return virtualPathData;
            }

            var content = _contentLoader.Get<IContent>(contentLink);
            if (ShouldGenerateProjectModeUrlPostfix(content))
            {
                var virtualPath = GetAccessibleVirtualPath(virtualPathData, content as PageData, language);
                virtualPathData = AppendGeneratedPostfix(virtualPath);
            }

            if (content is ImageData imageData)
            {
                virtualPathData = _externalReviewUrlGenerator.GetProxiedImageUrl(imageData.ContentLink);

            }

            return virtualPathData;
        }

        private bool ShouldGenerateProjectModeUrlPostfix(IContent content)
        {
            if (!_externalReviewState.IsInProjectReviewContext)
            {
                return false;
            }

            if (content is not PageData)
            {
                return false;
            }

            var externalReviewLinksRepository = ServiceLocator.Current.GetInstance<IExternalReviewLinksRepository>();
            var hasPinCode = externalReviewLinksRepository.HasPinCode(_externalReviewState.Token);
            var pageParentSite = _siteDefinitionResolver.GetByContent(content.ContentLink, false);
            return !hasPinCode || pageParentSite == SiteDefinition.Current;
        }

        /// <summary>
        /// Get a virtual path that can be accessed. If URL segment has been changed we need
        /// to return the original (published) version's URL segment for the routing to work.
        /// </summary>
        private string GetAccessibleVirtualPath(string url, PageData data, string language)
        {
            var virtualPath = url;
            var provider = this._providerManager.ProviderMap.GetProvider(data.ContentLink.ProviderName);
            var masterContent = (PageData)provider.GetScatteredContents(
                new[] { data.ContentLink.ToReferenceWithoutVersion() },
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

        public string GetUrl(ContentReference contentLink, string language, UrlResolverArguments urlResolverArguments)
        {
            var url = _defaultUrlResolver.GetUrl(contentLink, language, urlResolverArguments);
            if (!_externalReviewState.IsInExternalReviewContext || url == null)
            {
                return url;
            }

            return GetInternalUrl(contentLink, url);
        }

        public string GetUrl(UrlBuilder urlBuilderWithInternalUrl, UrlResolverArguments arguments)
        {
            var url = _defaultUrlResolver.GetUrl(urlBuilderWithInternalUrl, arguments);
            if (!_externalReviewState.IsInExternalReviewContext || url == null)
            {
                return url;
            }

            var linkMap = _permanentLinkMapper.Find(urlBuilderWithInternalUrl);
            return linkMap != null ? GetInternalUrl(linkMap.ContentReference, url) : url;
        }

        /// <summary>
        /// Appends project info or proxies the images through custom image handler to bypass security checks
        /// </summary>
        /// <param name="contentLink"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetInternalUrl(ContentReference contentLink, string url)
        {
            var content = _contentLoader.Get<IContent>(contentLink);
            if (ShouldGenerateProjectModeUrlPostfix(content))
            {
                return AppendGeneratedPostfix(url);
            }

            if (content is ImageData imageData)
            {
                return _externalReviewUrlGenerator.GetProxiedImageUrl(imageData.ContentLink);
            }

            return url;
        }

        public bool TryToPermanent(string url, out string permanentUrl)
        {
            return _defaultUrlResolver.TryToPermanent(url, out permanentUrl);
        }

        public ContentRouteData Route(UrlBuilder urlBuilder, RouteArguments routeArguments)
        {
            var contentRouteData = _defaultUrlResolver.Route(urlBuilder, routeArguments);

            if (contentRouteData.RemainingPath.StartsWith($"{_externalReviewOptions.Service.ContentPreviewUrl}/", StringComparison.CurrentCultureIgnoreCase))
            {
                // If we failed to route then it means that a start page in the same language does not exist and our partial routers will not be able to step in
                // We need to fallback to master language
                if (contentRouteData.Content == null)
                {
                    var masterLanguage = LanguageSelector.AutoDetect().LanguageBranch;
                    urlBuilder.Path = urlBuilder.Path.Replace($"/{contentRouteData.RouteLanguage}", $"/{masterLanguage}");
                    var masterRoutedData = _defaultUrlResolver.Route(urlBuilder, routeArguments);
                    return masterRoutedData;
                }
            }

            return contentRouteData;
        }

        private string AppendGeneratedPostfix(string url)
        {
            var urlBuilder = new UrlBuilder(UrlPath.Combine(url, _externalReviewOptions.Service.ContentPreviewUrl,
                _externalReviewState.Token));
            urlBuilder.QueryCollection.Add(PreviewGenerated, bool.TrueString);
            return urlBuilder.ToString();
        }
    }
}
