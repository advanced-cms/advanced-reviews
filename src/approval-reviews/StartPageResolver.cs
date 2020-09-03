using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AdvancedApprovalReviews
{
    public interface IStartPageUrlResolver
    {
        string GetUrl(ContentReference contentReference);
    }

    [ServiceConfiguration(typeof(IStartPageUrlResolver))]
    public class StartPageUrlResolver: IStartPageUrlResolver
    {
        private readonly UrlResolver _urlResolver;
        private readonly IContentLoader _contentLoader;
        private readonly Lazy<IEnumerable<ContentReference>> _startPages;

        public StartPageUrlResolver(ISiteDefinitionRepository siteDefinitionRepository, UrlResolver urlResolver, IContentLoader contentLoader)
        {
            _urlResolver = urlResolver;
            _contentLoader = contentLoader;
            _startPages = new Lazy<IEnumerable<ContentReference>>(() =>
            {
                return siteDefinitionRepository.List().Select(x => x.StartPage.ToReferenceWithoutVersion());
            });
        }

        public string GetUrl(ContentReference contentReference)
        {
            var startPage = _startPages.Value.FirstOrDefault(x => x == contentReference.ToReferenceWithoutVersion());
            if (startPage != null)
            {
                return _urlResolver.GetUrl(startPage);
            }
            var ancestorLinks = _contentLoader.GetAncestors(contentReference)
                .Select(x => x.ContentLink.ToReferenceWithoutVersion());
            startPage = _startPages.Value.FirstOrDefault(x => ancestorLinks.Contains(x));
            return _urlResolver.GetUrl(startPage ?? ContentReference.StartPage);
        }
    }
}
