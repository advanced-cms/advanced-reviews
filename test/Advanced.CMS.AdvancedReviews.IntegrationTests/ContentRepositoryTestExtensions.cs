using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public static class ContentRepositoryTestExtensions
{
    private static IContentRepository ContentRepository => ServiceLocator.Current.GetInstance<IContentRepository>();

    private static IPropertyDefinitionRepository PropertyDefinitionRepository =>
        ServiceLocator.Current.GetInstance<IPropertyDefinitionRepository>();

    private static IContentTypeRepository ContentTypeRepository =>
        ServiceLocator.Current.GetInstance<IContentTypeRepository>();

    private static IContentSecurityRepository ContentSecurityRepository =>
        ServiceLocator.Current.GetInstance<IContentSecurityRepository>();

    private static IExternalReviewLinksRepository ExternalReviewLinksRepository =>
        ServiceLocator.Current.GetInstance<IExternalReviewLinksRepository>();

    public static StandardPage CreatePage(this IContentRepository repo, string name = null)
    {
        var page = repo.GetDefault<StandardPage>(ContentReference.StartPage);
        page.PageName = name ?? Guid.NewGuid().ToString();
        repo.Save(page, AccessLevel.NoAccess);
        return page;
    }

    public static StandardPage WithoutEveryoneAccess(this StandardPage page)
    {
        var accessControlList = new AccessControlList
        {
            new AccessControlEntry(EPiServer.Authorization.Roles.CmsAdmins, AccessLevel.Administer)
        };
        accessControlList.IsInherited = false;
        page.SaveSecurityInfo(ContentSecurityRepository, accessControlList, SecuritySaveType.Replace);
        return page;
    }

    public static ExternalReviewLink GenerateExternalReviewLink(this StandardPage page, Project project = null)
    {
        return ExternalReviewLinksRepository.AddLink(page.ContentLink, false, TimeSpan.FromDays(1), project?.ID);
    }

    public static ExternalReviewLink ExpireReviewLink(this ExternalReviewLink externalReviewLink)
    {
        return ExternalReviewLinksRepository.UpdateLink(externalReviewLink.Token,
            DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), null, null, null);
    }

    public static StandardPage PublishPage(this StandardPage page)
    {
        ContentRepository.Publish(page, AccessLevel.NoAccess);
        return page;
    }

    public static ContentReference CreateTargetFolder(this IContentRepository repo)
    {
        var folder = repo.GetDefault<ContentFolder>(ContentReference.GlobalBlockFolder);
        folder.Name = Guid.NewGuid().ToString();
        repo.Save(folder, AccessLevel.NoAccess);
        return folder.ContentLink;
    }

    public static Task CleanupAsync(this IContentRepository repo, IContent page)
    {
        repo.Delete(page.ContentLink, true, AccessLevel.NoAccess);
        foreach (var contentType in ContentTypeRepository.List())
        {
            ResetContentType(contentType);
        }

        return Task.CompletedTask;
    }

    private static void ResetContentType(ContentType contentType)
    {
        var writableContentType = contentType.CreateWritableClone() as ContentType;
        foreach (var property in writableContentType.PropertyDefinitions)
        {
            if (property.ExistsOnModel)
            {
                continue;
            }

            PropertyDefinitionRepository.Delete(property);
        }

        writableContentType.ResetContentType();
        ContentTypeRepository.Save(writableContentType);
    }
}
