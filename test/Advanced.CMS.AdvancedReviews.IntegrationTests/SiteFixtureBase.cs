using System;
using System.IO;
using System.Net.Http;
using Advanced.CMS.ExternalReviews;
using Advanced.CMS.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using TestSite;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests
{
    public class SiteFixtureBase : IDisposable
    {
        private readonly UIServiceFixture<Startup> _serviceFixture;
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
                _serviceFixture.CreateClient();
            }
            catch
            {
                Dispose();
            }
        }

        private string EnsureDatabase()
        {
            var dbFile = Path.Combine(Environment.CurrentDirectory, @"../../../sites/TestSite/App_Data/cms.mdf");
            _databaseFixture = new CmsDatabaseFixture(Path.Combine(Environment.CurrentDirectory, @"../../Assets/db_template.mdf"), dbFile);
            var connectionString = $"Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog={_databaseFixture.DatabaseName};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
            var dbHelper = new DatabaseHelper(connectionString);
            var aspNetIdentitySchema = SolutionPathUtility.GetSolutionPath(@"test\Advanced.CMS.IntegrationTests\IdentitySchema.sql", "Advanced.CMS.AdvancedReviews.sln");
            dbHelper.ExecuteSqlFile(aspNetIdentitySchema);

            return connectionString;
        }

        public IServiceProvider Services => _serviceFixture.Services;
        public HttpClient Client => _serviceFixture.CreateClient();
        public HttpClient NonRedirectClient => _serviceFixture.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        public void Dispose()
        {
            _serviceFixture.Dispose();
            _databaseFixture.Dispose();
        }
    }
}
