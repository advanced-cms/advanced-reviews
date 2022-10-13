using System;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using Advanced.CMS.IntegrationTests.ServiceMocks;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using TestSite.Models;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class TestScenarioBuilder
{
    public const string OriginalBlockContent = "_ORIGINAL_BLOCK_CONTENT";
    public const string UpdatedString = "_UPDATED_OUTSIDE_OF_PROJECT_MODE";
    public const string ProjectUpdatedString = "_UPDATED_IN_PROJECT_MODE";

    private readonly IContentRepository _contentRepository;
    private readonly IContentSecurityRepository _contentSecurityRepository;
    private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
    private readonly ProjectRepository _projectRepository;
    private readonly ProjectService _projectService;
    private readonly IContentVersionRepository _contentVersionRepository;
    private readonly MockableCurrentProject _currentProject;

    private StandardPage _page;
    private ExternalReviewLink _externalReviewLink;
    private Project _project;
    private bool _isInProject;
    private ContentReference _blockReference;

    public TestScenarioBuilder(IContentRepository contentRepository,
        IContentSecurityRepository contentSecurityRepository,
        IExternalReviewLinksRepository externalReviewLinksRepository,
        ProjectRepository projectRepository, MockableCurrentProject currentProject, ProjectService projectService,
        IContentVersionRepository contentVersionRepository)
    {
        _contentRepository = contentRepository;
        _contentSecurityRepository = contentSecurityRepository;
        _externalReviewLinksRepository = externalReviewLinksRepository;
        _projectRepository = projectRepository;
        _currentProject = currentProject;
        _projectService = projectService;
        _contentVersionRepository = contentVersionRepository;
    }

    public TestEnvironment Build()
    {
        return new TestEnvironment(_page, _externalReviewLink);
    }

    public TestScenarioBuilder Init()
    {
        var page = _contentRepository.GetDefault<StandardPage>(ContentReference.StartPage);
        page.PageName = Guid.NewGuid().ToString();
        _contentRepository.Save(page, AccessLevel.NoAccess);
        _page = page;
        return this;
    }

    public TestScenarioBuilder AddBlock()
    {
        var block = _contentRepository.GetDefault<EditorialBlock>(ContentReference.GlobalBlockFolder);
        block.MainBody = Guid.NewGuid().ToString();
        var blockContent = block as IContent;
        blockContent.Name = Guid.NewGuid().ToString();
        block.MainBody = OriginalBlockContent ;
        _blockReference = _contentRepository.Save(blockContent, AccessLevel.NoAccess).ToReferenceWithoutVersion();
        var contentAreaItem = new ContentAreaItem
        {
            ContentLink = blockContent.ContentLink,
            ContentGuid = blockContent.ContentGuid
        };
        var contentArea = new ContentArea();
        contentArea.Items.Add(contentAreaItem);
        _page.ContentArea = contentArea;
        _contentRepository.Save(_page, AccessLevel.NoAccess);

        var versions = _contentVersionRepository.List(_blockReference);

        return this;
    }

    public TestScenarioBuilder PublishPage()
    {
        _contentRepository.Publish(_page, AccessLevel.NoAccess);
        return this;
    }

    public TestScenarioBuilder UpdateBlock()
    {
        using (var unitOfWork = AdminUnitOfWork.Begin())
        {
            var block = _contentRepository.Get<EditorialBlock>(_blockReference).CreateWritableClone() as EditorialBlock;
            var blockContent = block as IContent;
            if (_isInProject)
            {
                block.MainBody = block.MainBody.Replace(OriginalBlockContent, ProjectUpdatedString)
                    .Replace(UpdatedString, ProjectUpdatedString);
                _contentRepository.Save(blockContent, SaveAction.Save | SaveAction.ForceNewVersion);
                _projectService.AddToCurrentProject(new [] { blockContent.ContentLink });
            }
            else
            {
                block.MainBody = block.MainBody.Replace(OriginalBlockContent, UpdatedString)
                    .Replace(ProjectUpdatedString, UpdatedString);
                var draft = _contentVersionRepository.LoadCommonDraft(_blockReference,
                    LanguageSelector.AutoDetect().LanguageBranch);
                if (draft.IsCommonDraft)
                {
                    _contentRepository.Save(blockContent, SaveAction.Save);
                }
                else
                {
                    _contentRepository.Save(blockContent, SaveAction.Save | SaveAction.ForceNewVersion);
                }
            }
        }

        return this;
    }

    public TestScenarioBuilder WithoutEveryoneAccess()
    {
        var accessControlList = new AccessControlList
        {
            new AccessControlEntry(EPiServer.Authorization.Roles.CmsAdmins, AccessLevel.Administer)
        };
        accessControlList.IsInherited = false;
        _page.SaveSecurityInfo(this._contentSecurityRepository, accessControlList, SecuritySaveType.Replace);
        return this;
    }

    public TestScenarioBuilder WithViewPin()
    {
        _externalReviewLink = _externalReviewLinksRepository.AddLink(_page.ContentLink, false, TimeSpan.FromDays(1),
            _currentProject.ProjectId);
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

            _projectRepository.Save(_project);
            _currentProject.SetProject(_project.ID);
            _isInProject = true;
        }

        return this;
    }

    public TestScenarioBuilder SwitchToCommonDrafts()
    {
        _currentProject.SetProject(null);
        _isInProject = false;
        return this;
    }
}
