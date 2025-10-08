using System.Net;
using Advanced.CMS.AdvancedReviews.IntegrationTests.Tooling;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.ServiceLocation;
using TestSite.Models;
using Xunit;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.Projects;

[Collection(IntegrationTestCollection.Name)]
public class When_Creating_Token_For_Page_In_Project_Mode(When_Creating_Token_For_Page_In_Project_Mode.TestFixture fixture)
    : IClassFixture<CommonFixture>, IClassFixture<When_Creating_Token_For_Page_In_Project_Mode.TestFixture>
{
    public class TestFixture(SiteFixture siteFixture) : IAsyncLifetime
    {
        public IContentRepository ContentRepository { get; } = siteFixture.Services.GetInstance<IContentRepository>();
        public ProjectRepository ProjectRepository { get; } = siteFixture.Services.GetInstance<ProjectRepository>();
        public IServiceProvider Services => siteFixture.Services;
        public HttpClient Client { get; } = siteFixture.Client;

        public StandardPage Page { get; private set; }
        public ExternalReviewLink CommonDraftReviewLink { get; set; }

        public async Task InitializeAsync()
        {
            Page = ContentRepository.CreatePage().WithoutEveryoneAccess().AddBlock();
            CommonDraftReviewLink = Page.GenerateExternalReviewLink();

            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => await ContentRepository.CleanupAsync(Page);
    }

    [Fact]
    public async Task It_Returns_200()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.CommonDraftReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Should_Load_Common_Draft()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, fixture.CommonDraftReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.Contains(StaticTexts.OriginalBlockContent, responseText);
    }

    [Fact]
    public async Task When_Creating_New_Version_In_Project_Mode_It_Should_Load_Project_Specific_Draft()
    {
        var project = GetProject();
        var projectExternalReviewLink = fixture.Page.UpdateBlock(project).GenerateExternalReviewLink(project);

        var message = new HttpRequestMessage(HttpMethod.Get, projectExternalReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.Contains(StaticTexts.ProjectUpdatedString, responseText);
    }

    [Fact]
    public async Task When_Modifying_Common_Draft_It_Should_Load_Correct_Updated_String()
    {
        var updatedReviewLink = fixture.Page.UpdateBlock().GenerateExternalReviewLink();

        var message = new HttpRequestMessage(HttpMethod.Get, updatedReviewLink.LinkUrl);
        var response = await fixture.Client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.Contains(StaticTexts.UpdatedString, responseText);
    }

    private Project GetProject()
    {
        var project = new Project
        {
            Name = Guid.NewGuid().ToString()
        };

        fixture.ProjectRepository.Save(project);
        return project;
    }
}

