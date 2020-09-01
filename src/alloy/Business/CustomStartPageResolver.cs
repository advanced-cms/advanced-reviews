using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedApprovalReviews;
using AdvancedExternalReviews;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AlloyTemplates.Business
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //Implementations for custom interfaces can be registered here.

            context.ConfigurationComplete += (o, e) =>
                {
                    // do not register for for this site

                    //context.Services.AddTransient<IStartPageUrlResolver, CustomStartPageResolver>();
                };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }

    /// <summary>
    /// example of custom IStartPageUrlResolver
    /// it can be used for example when StartPage has assigned shortcut
    /// </summary>
    public class CustomStartPageResolver: IStartPageUrlResolver
    {
        private readonly UrlResolver _urlResolver;
        private readonly IContentLoader _contentLoader;
        private Lazy<List<SiteDefinition>> _siteDefinitions;

        public CustomStartPageResolver(ISiteDefinitionRepository siteDefinitionRepository, UrlResolver urlResolver, IContentLoader contentLoader)
        {
            _urlResolver = urlResolver;
            _contentLoader = contentLoader;

            _siteDefinitions = new Lazy<List<SiteDefinition>>(() => siteDefinitionRepository.List().ToList());
        }

        private string ResolveUrlForSite(SiteDefinition siteDefinition)
        {
            var host = siteDefinition.Hosts.FirstOrDefault(x => x.Type == HostDefinitionType.Primary);
            if (host != null)
            {
                return UrlPath.Combine(host.Url.ToString(), ContentLanguage.PreferredCulture.Name);
            }

            host = siteDefinition.Hosts.FirstOrDefault();
            if (host == null)
            {
                throw new ApplicationException("No host for defined for site");
            }

            return UrlPath.Combine(host.Url.ToString(), ContentLanguage.PreferredCulture.Name);
        }

        public string GetUrl(ContentReference contentReference)
        {
            var siteDefinition = _siteDefinitions.Value.FirstOrDefault(x => x.StartPage.ToReferenceWithoutVersion() == contentReference.ToReferenceWithoutVersion());
            if (siteDefinition != null)
            {
                return ResolveUrlForSite(siteDefinition);
            }
            var ancestorLinks = _contentLoader.GetAncestors(contentReference)
                .Select(x => x.ContentLink.ToReferenceWithoutVersion());

            siteDefinition = _siteDefinitions.Value.FirstOrDefault(x => ancestorLinks.Contains(x.StartPage.ToReferenceWithoutVersion()));
            if (siteDefinition == null)
            {
                throw new ApplicationException("No site found for " + contentReference);
            }

            return ResolveUrlForSite(siteDefinition);
        }
    }
}
