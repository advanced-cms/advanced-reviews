﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Advanced.CMS.ExternalReviews;
using Advanced.CMS.IntegrationTests;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestSite;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests;

public class SiteFixtureBase : IDisposable
{
    private UIServiceFixture<Startup> _serviceFixture;
    private CmsDatabaseFixture _databaseFixture;

    public SiteFixtureBase(Action<ExternalReviewOptions> optionsCallback = null)
    {
        string connectionString = EnsureDatabase();
        _serviceFixture = new UIServiceFixture<Startup>(connectionString, collection =>
        {
            if (optionsCallback != null)
            {
                collection.Configure(optionsCallback);
            }
        });
        try
        {
            Client = _serviceFixture.CreateClient();
        }
        catch
        {
            Dispose();
        }
    }

    private string EnsureDatabase()
    {
        var dbFile = Path.GetFullPath(@"..\..\..\..\sites\TestSite\App_Data\cms.mdf", Environment.CurrentDirectory);
        var databaseMdfTemplateFile = Path.GetFullPath(@"..\..\..\..\Advanced.CMS.AdvancedReviews.IntegrationTests\Assets\db_template.mdf", Environment.CurrentDirectory);
        _databaseFixture = new CmsDatabaseFixture(databaseMdfTemplateFile, dbFile);
        var connectionString =
            $"Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog={_databaseFixture.DatabaseName};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
        var dbHelper = new DatabaseHelper(connectionString);
        var aspNetIdentitySchema = SolutionPathUtility.GetSolutionPath(
            @"test\Advanced.CMS.IntegrationTests\IdentitySchema.sql", "Advanced.CMS.AdvancedReviews.sln");
        dbHelper.ExecuteSqlFile(aspNetIdentitySchema);

        return connectionString;
    }

    public IServiceProvider Services => _serviceFixture.Services;
    public HttpClient Client { get; set; }

    public void Dispose()
    {
        foreach (var hostedService in Services.GetAllInstances<IHostedService>().ToList())
        {
            hostedService.StopAsync(new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        Client.Dispose();
        _serviceFixture.Dispose();
        _databaseFixture.Dispose();
    }
}
