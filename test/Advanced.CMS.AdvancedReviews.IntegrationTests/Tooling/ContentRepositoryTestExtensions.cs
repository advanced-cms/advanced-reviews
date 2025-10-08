using System.Globalization;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;

public static class ContentRepositoryTestExtensions
{
    private static IContentRepository ContentRepository => ServiceLocator.Current.GetInstance<IContentRepository>();

    private static IContentVersionRepository ContentVersionRepository => ServiceLocator.Current.GetInstance<IContentVersionRepository>();

    private static ProjectRepository ProjectRepository => ServiceLocator.Current.GetInstance<ProjectRepository>();

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

    public static StandardPage AddBlock(this StandardPage page)
    {
        var block = ContentRepository.GetDefault<EditorialBlock>(ContentReference.GlobalBlockFolder);
        block.MainBody = Guid.NewGuid().ToString();
        var blockContent = block as IContent;
        blockContent.Name = Guid.NewGuid().ToString();
        block.MainBody = StaticTexts.OriginalBlockContent;
        ContentRepository.Save(blockContent, AccessLevel.NoAccess).ToReferenceWithoutVersion();
        var contentAreaItem = new ContentAreaItem
        {
            ContentLink = blockContent.ContentLink,
        };
        var contentArea = new ContentArea();
        contentArea.Items.Add(contentAreaItem);
        page.ContentArea = contentArea;
        ContentRepository.Save(page, AccessLevel.NoAccess);
        return page;
    }

    public static StandardPage UpdateBlock(this StandardPage page, Project project = null)
    {
        var _blockReference = page.ContentArea.Items.FirstOrDefault().ContentLink;

        var block = ContentRepository.Get<EditorialBlock>(_blockReference).CreateWritableClone() as EditorialBlock;
        var blockContent = block as IContent;
        if (project != null)
        {
            block.MainBody = block.MainBody.Replace(StaticTexts.OriginalBlockContent, StaticTexts.ProjectUpdatedString)
                .Replace(StaticTexts.UpdatedString, StaticTexts.ProjectUpdatedString);
            ContentRepository.Save(blockContent, SaveAction.Save, AccessLevel.NoAccess);
            var projectItem = new ProjectItem
            {
                ProjectID = project.ID,
                ContentLink = blockContent.ContentLink,
                Language = blockContent is ILocalizable localizable
                    ? localizable.Language
                    : CultureInfo.InvariantCulture,
                Category = "default"
            };

            ProjectRepository.SaveItems([projectItem]);
        }
        else
        {
            block.MainBody = block.MainBody.Replace(StaticTexts.OriginalBlockContent, StaticTexts.UpdatedString)
                .Replace(StaticTexts.ProjectUpdatedString, StaticTexts.UpdatedString);
            var draft = ContentVersionRepository.LoadCommonDraft(_blockReference,
                LanguageSelector.AutoDetect().LanguageBranch);
            if (draft.IsCommonDraft)
            {
                ContentRepository.Save(blockContent, SaveAction.Save, AccessLevel.NoAccess);
            }
            else
            {
                ContentRepository.Save(blockContent, SaveAction.Save | SaveAction.ForceNewVersion,
                    AccessLevel.NoAccess);
            }
        }

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

        var projects = ProjectRepository.List().ToList();

        foreach (var project in projects)
        {
            ProjectRepository.Delete(project.ID);
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
