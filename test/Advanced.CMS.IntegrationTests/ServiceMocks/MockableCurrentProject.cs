﻿using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

[ServiceConfiguration(typeof (CurrentProject))]
[ServiceConfiguration(typeof (MockableCurrentProject))]
[ServiceConfiguration(typeof (CurrentProject), Lifecycle = ServiceInstanceScope.Singleton)]
public class MockableCurrentProject : CurrentProject
{
    private int? projectId;

    public MockableCurrentProject(IHttpContextAccessor httpContextAccessor, ServiceAccessor<ProjectRepository> projectRepositoryAccessor) : base(httpContextAccessor, projectRepositoryAccessor)
    {
    }

    public override int? ProjectId => projectId;

    public void SetProject(int? value)
    {
        projectId = value;
    }
}
