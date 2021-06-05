using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AdvancedApprovalReviews
{
    public interface IStartPageUrlResolver
    {
        string GetUrl(ContentReference contentReference, string languageBranch = null);
    }

    [ServiceConfiguration(typeof(IStartPageUrlResolver))]
    public class StartPageUrlResolver: IStartPageUrlResolver
    {
        private readonly IUrlResolver _urlResolver;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;

        public StartPageUrlResolver(IUrlResolver urlResolver, ISiteDefinitionResolver siteDefinitionResolver)
        {
            _urlResolver = urlResolver;
            _siteDefinitionResolver = siteDefinitionResolver;
        }

        public string GetUrl(ContentReference contentReference, string languageBranch = null)
        {
            var site = _siteDefinitionResolver.GetByContent(contentReference, true);
            return _urlResolver.GetUrl(site?.StartPage ?? ContentReference.StartPage, languageBranch);
        }
    }
}
