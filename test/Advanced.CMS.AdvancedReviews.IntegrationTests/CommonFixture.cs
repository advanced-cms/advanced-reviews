using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class CommonFixture : IDisposable
{
    private readonly ISiteDefinitionRepository siteDefinitionRepository;
    private readonly SiteDefinition site;
    private readonly IContentRepository contentRepository;
    private readonly StartPage start;

    public CommonFixture()
    {
        contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();

        var sysDef = ServiceLocator.Current.GetInstance<SystemDefinition>();
        start = contentRepository.GetDefault<StartPage>(sysDef.RootPage);
        start.Name = "Start";
        var startPage = contentRepository.Save(start, SaveAction.Publish, AccessLevel.NoAccess)
            .ToReferenceWithoutVersion();

        site = new SiteDefinition
        {
            Name = "Start", StartPage = startPage, SiteUrl = new Uri("http://localhost/"),
            Hosts = { new HostDefinition { Name = "*" } }
        };
        siteDefinitionRepository.Save(site);
    }

    public void Dispose()
    {
        siteDefinitionRepository.Delete(site.Id);
        contentRepository.Delete(start.ContentLink, true, AccessLevel.NoAccess);
    }
}
