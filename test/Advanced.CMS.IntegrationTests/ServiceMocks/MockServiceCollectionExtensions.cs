using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Cms.Shell.UI.Rest.Projects.Internal;
using EPiServer.Core.Internal;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.IntegrationTests.ServiceMocks;

public static class MockServiceCollectionExtensions
{
    public static IServiceCollection MockServices(this IServiceCollection services)
    {
        var currentProjectServiceDescriptor = services.First(s => s.ServiceType == typeof(CurrentProject));
        services.Remove(currentProjectServiceDescriptor);
        services.AddSingleton<CurrentProject, MockableCurrentProject>();
        services.AddSingleton<MockableCurrentProject>();

        var projectLoaderServiceServiceDescriptor = services.First(s => s.ServiceType == typeof(ProjectLoaderService));
        services.Remove(projectLoaderServiceServiceDescriptor);
        services.AddTransient<ProjectLoaderService, MockableProjectLoaderService>();
        services.AddTransient<MockableProjectLoaderService>();

        var projectServiceServiceDescriptor = services.First(s => s.ServiceType == typeof(ProjectService));
        services.Remove(projectServiceServiceDescriptor);
        services.AddTransient<ProjectService, MockableProjectService>();
        services.AddTransient<MockableProjectService>();

        var contentLoaderServiceDescriptor = services.First(s => s.ServiceType == typeof(ContentLoaderService));
        services.Remove(contentLoaderServiceDescriptor);
        services.AddSingleton<ContentLoaderService, MockableContentLoaderService>();
        services.AddSingleton<MockableContentLoaderService>();

        var contentServiceDescriptor = services.First(s => s.ServiceType == typeof(ContentService));
        services.Remove(contentServiceDescriptor);
        services.AddSingleton<ContentService, MockableContentService>();
        services.AddSingleton<MockableContentService>();

        var contentAccessCheckerServiceDescriptor = services.First(s => s.ServiceType == typeof(ContentAccessChecker));
        services.Remove(contentAccessCheckerServiceDescriptor);
        services.AddSingleton<ContentAccessChecker, MockableContentAccessChecker>();
        services.AddSingleton<MockableContentAccessChecker>();

        var contentRepositoryServiceDescriptor = services.First(s => s.ServiceType == typeof(ContentRepository));
        services.Remove(contentRepositoryServiceDescriptor);
        var interfaceContentRepositoryServiceDescriptor =
            services.First(s => s.ServiceType == typeof(IContentRepository));
        services.Remove(interfaceContentRepositoryServiceDescriptor);
        services.AddSingleton<IContentRepository, MockableContentRepository>();
        services.AddSingleton<ContentRepository, MockableContentRepository>();
        services.AddSingleton<MockableContentRepository>();

        return services;
    }
}

[ServiceConfiguration]
public class AdminUnitOfWork(
    MockableContentLoaderService mockableContentLoaderService,
    MockableContentService mockableContentService,
    MockableContentAccessChecker mockableContentAccessChecker)
    : IDisposable
{
    private readonly MockableContentLoaderService _mockableContentLoaderService = mockableContentLoaderService;
    private readonly MockableContentService _mockableContentService = mockableContentService;
    private readonly MockableContentAccessChecker _mockableContentAccessChecker = mockableContentAccessChecker;

    public static AdminUnitOfWork Begin()
    {
        var adminContext = ServiceLocator.Current.GetInstance<AdminUnitOfWork>();
        adminContext._mockableContentLoaderService.Enabled = true;
        adminContext._mockableContentService.Enabled = true;
        adminContext._mockableContentAccessChecker.Enabled = true;
        return adminContext;
    }

    public void Dispose()
    {
        _mockableContentLoaderService.Enabled = false;
        _mockableContentService.Enabled = false;
        _mockableContentAccessChecker.Enabled = false;
    }
}
