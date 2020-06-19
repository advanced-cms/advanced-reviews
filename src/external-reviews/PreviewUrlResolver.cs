using System.Collections.Specialized;
using System.Web.Routing;
using EPiServer;
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

        public PreviewUrlResolver(UrlResolver defaultUrlResolver, IContentLoader contentLoader, IPermanentLinkMapper permanentLinkMapper)
        {
            _defaultUrlResolver = defaultUrlResolver;
            _contentLoader = contentLoader;
            _permanentLinkMapper = permanentLinkMapper;
        }

        public override IContent Route(UrlBuilder urlBuilder, ContextMode contextMode)
        {
            return _defaultUrlResolver.Route(urlBuilder, contextMode);
        }

        public static bool IsGenerated(NameValueCollection queryString)
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
            if (content is PageData)
            {
                virtualPathData.VirtualPath = AppendGeneratedPostfix(virtualPathData.VirtualPath);
            }

            return virtualPathData;
        }

        public override string GetUrl(UrlBuilder urlBuilderWithInternalUrl, VirtualPathArguments arguments)
        {
            var url = _defaultUrlResolver.GetUrl(urlBuilderWithInternalUrl, arguments);
            if (!ExternalReview.IsInProjectReviewContext || !ExternalReview.IsInExternalReviewContext || url == null)
            {
                return url;
            }

            var linkMap = _permanentLinkMapper.Find(urlBuilderWithInternalUrl);
            var content = _contentLoader.Get<IContent>(linkMap.ContentReference);
            if (content is PageData)
            {
                return AppendGeneratedPostfix(url);
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
            return $"{url.TrimEnd('/')}/{_externalReviewOptions.Service.ContentPreviewUrl}/{ExternalReview.Token}?{PreviewGenerated}=true";
        }
    }
}
