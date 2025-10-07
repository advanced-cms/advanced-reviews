using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using Advanced.CMS.IntegrationTests.ServiceMocks;
using EPiServer.Authorization;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.DataAccess;
using EPiServer.Security;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class TestScenarioBuilder(
    IContentRepository contentRepository,
    IContentSecurityRepository contentSecurityRepository,
    IExternalReviewLinksRepository externalReviewLinksRepository,
    ProjectRepository projectRepository,
    MockableCurrentProject currentProject,
    ProjectService projectService,
    IContentVersionRepository contentVersionRepository)
{
    public const string OriginalBlockContent = "_ORIGINAL_BLOCK_CONTENT";
    public const string UpdatedString = "_UPDATED_OUTSIDE_OF_PROJECT_MODE";
    public const string ProjectUpdatedString = "_UPDATED_IN_PROJECT_MODE";

    private StandardPage _page;
    private ExternalReviewLink _externalReviewLink;
    private Project _project;
    private bool _isInProject;
    private ContentReference _blockReference;

    public TestEnvironment Build()
    {
        return new TestEnvironment(_page, _externalReviewLink);
    }

    public TestScenarioBuilder Init()
    {
        var page = contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
        page.PageName = Guid.NewGuid().ToString();
        contentRepository.Save(page, AccessLevel.NoAccess);
        _page = page;
        return this;
    }

    public TestScenarioBuilder AddBlock()
    {
        var block = contentRepository.GetDefault<EditorialBlock>(ContentReference.GlobalBlockFolder);
        block.MainBody = Guid.NewGuid().ToString();
        var blockContent = block as IContent;
        blockContent.Name = Guid.NewGuid().ToString();
        block.MainBody = OriginalBlockContent ;
        _blockReference = contentRepository.Save(blockContent, AccessLevel.NoAccess).ToReferenceWithoutVersion();
        var contentAreaItem = new ContentAreaItem
        {
            ContentLink = blockContent.ContentLink,
            ContentGuid = blockContent.ContentGuid
        };
        var contentArea = new ContentArea();
        contentArea.Items.Add(contentAreaItem);
        _page.ContentArea = contentArea;
        contentRepository.Save(_page, AccessLevel.NoAccess);

        var versions = contentVersionRepository.List(_blockReference);

        return this;
    }

    public TestScenarioBuilder PublishPage()
    {
        contentRepository.Publish(_page, AccessLevel.NoAccess);
        return this;
    }

    public TestScenarioBuilder UpdateBlock()
    {
        using (var unitOfWork = AdminUnitOfWork.Begin())
        {
            var block = contentRepository.Get<EditorialBlock>(_blockReference).CreateWritableClone() as EditorialBlock;
            var blockContent = block as IContent;
            if (_isInProject)
            {
                block.MainBody = block.MainBody.Replace(OriginalBlockContent, ProjectUpdatedString)
                    .Replace(UpdatedString, ProjectUpdatedString);
                contentRepository.Save(blockContent, SaveAction.Save | SaveAction.ForceNewVersion);
                projectService.AddToCurrentProject(new [] { blockContent.ContentLink });
            }
            else
            {
                block.MainBody = block.MainBody.Replace(OriginalBlockContent, UpdatedString)
                    .Replace(ProjectUpdatedString, UpdatedString);
                var draft = contentVersionRepository.LoadCommonDraft(_blockReference,
                    LanguageSelector.AutoDetect().LanguageBranch);
                if (draft.IsCommonDraft)
                {
                    contentRepository.Save(blockContent, SaveAction.Save);
                }
                else
                {
                    contentRepository.Save(blockContent, SaveAction.Save | SaveAction.ForceNewVersion);
                }
            }
        }

        return this;
    }

    public TestScenarioBuilder WithoutEveryoneAccess()
    {
        var accessControlList = new AccessControlList
        {
            new AccessControlEntry(Roles.CmsAdmins, AccessLevel.Administer)
        };
        accessControlList.IsInherited = false;
        _page.SaveSecurityInfo(contentSecurityRepository, accessControlList, SecuritySaveType.Replace);
        return this;
    }

    public TestScenarioBuilder WithViewPin()
    {
        _externalReviewLink = externalReviewLinksRepository.AddLink(_page.ContentLink, false, TimeSpan.FromDays(1),
            currentProject.ProjectId);
        return this;
    }

    public TestScenarioBuilder PinExpired()
    {
        externalReviewLinksRepository.UpdateLink(_externalReviewLink.Token,
            DateTime.Now.Subtract(TimeSpan.FromSeconds(1)),
            null, null,
            null);
        return this;
    }

    public void Initialize(TestEnvironment testEnvironment)
    {
        this._page = testEnvironment.Page;
        this._externalReviewLink = testEnvironment.ExternalReviewLink;
    }

    public TestScenarioBuilder SwitchToProject()
    {
        if (_project == null)
        {
            _project = new Project
            {
                Name = Guid.NewGuid().ToString()
            };

            projectRepository.Save(_project);
            currentProject.SetProject(_project.ID);
            _isInProject = true;
        }

        return this;
    }

    public TestScenarioBuilder SwitchToCommonDrafts()
    {
        currentProject.SetProject(null);
        _isInProject = false;
        return this;
    }
}
