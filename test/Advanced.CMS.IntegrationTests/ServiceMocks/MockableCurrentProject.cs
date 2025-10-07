using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public class MockableCurrentProject(
    IHttpContextAccessor httpContextAccessor,
    ServiceAccessor<ProjectRepository> projectRepositoryAccessor)
    : CurrentProject(httpContextAccessor,
        projectRepositoryAccessor)
{
    private int? projectId;

    public override int? ProjectId => projectId;

    public void SetProject(int? value)
    {
        projectId = value;
    }
}
