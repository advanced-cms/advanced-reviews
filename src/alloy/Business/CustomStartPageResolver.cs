using System;
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
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;

        public CustomStartPageResolver(ISiteDefinitionResolver siteDefinitionResolver)
        {
            _siteDefinitionResolver = siteDefinitionResolver;
        }

        private string ResolveUrlForSite(SiteDefinition siteDefinition, string languageBranch)
        {
            var host = siteDefinition.Hosts.FirstOrDefault(x => x.Type == HostDefinitionType.Primary);
            if (host != null)
            {
                return UrlPath.Combine(host.Url.ToString(), languageBranch);
            }

            host = siteDefinition.Hosts.FirstOrDefault();
            if (host == null)
            {
                throw new ApplicationException("No host for defined for site");
            }

            return UrlPath.Combine(host.Url.ToString(), languageBranch);
        }

        public string GetUrl(ContentReference contentReference, string languageBranch)
        {
            var siteDefinition = _siteDefinitionResolver.GetByContent(contentReference, true);
            if (siteDefinition == null)
            {
                throw new ApplicationException("No site found for " + contentReference);
            }
            return ResolveUrlForSite(siteDefinition, languageBranch);
        }
    }
}
