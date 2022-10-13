using System;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

[ServiceConfiguration]
public class TestScenarioBuilder
{
    private readonly IContentRepository _contentRepository;
    private readonly IContentSecurityRepository _contentSecurityRepository;
    private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;

    private PageData _pageData;
    private ExternalReviewLink _externalReviewLink;

    public TestScenarioBuilder(IContentRepository contentRepository,
        IContentSecurityRepository contentSecurityRepository,
        IExternalReviewLinksRepository externalReviewLinksRepository)
    {
        _contentRepository = contentRepository;
        _contentSecurityRepository = contentSecurityRepository;
        _externalReviewLinksRepository = externalReviewLinksRepository;
    }

    public TestScenarioBuilder Reset()
    {
        var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
        page.PageName = "test page";
        _contentRepository.Save(page, AccessLevel.NoAccess);
        _pageData = page;
        return this;
    }

    public TestEnvironment Build()
    {
        return new TestEnvironment(_pageData, _externalReviewLink);
    }

    public TestScenarioBuilder WithoutEveryoneAccess()
    {
        var accessControlList = new AccessControlList
        {
            new AccessControlEntry(EPiServer.Authorization.Roles.CmsAdmins, AccessLevel.Administer)
        };
        accessControlList.IsInherited = false;
        _pageData.SaveSecurityInfo(this._contentSecurityRepository, accessControlList, SecuritySaveType.Replace);
        return this;
    }

    public TestScenarioBuilder WithViewPin()
    {
        _externalReviewLink = _externalReviewLinksRepository.AddLink(_pageData.ContentLink, false, TimeSpan.FromDays(1), null);
        return this;
    }

    public TestScenarioBuilder PinExpired()
    {
        _externalReviewLinksRepository.UpdateLink(_externalReviewLink.Token,
            DateTime.Now.Subtract(TimeSpan.FromSeconds(1)),
            null, null,
            null);
        return this;
    }
}
