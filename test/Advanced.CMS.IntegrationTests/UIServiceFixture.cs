using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.IntegrationTests
{
    public class UIServiceFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private string _connectionString;
        private readonly Action<IServiceCollection> _customRegistrations;

        public UIServiceFixture(string connectionString, Action<IServiceCollection> customRegistrations = null)
        {
            _connectionString = connectionString;
            _customRegistrations = customRegistrations;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["ConnectionStrings:EPiServerDB"] = _connectionString
                    });
            });
            builder.ConfigureServices(context =>
            {
                //Get existing service registration for DatabaseModeService
                var existingServiceDefinition = context.Single(x => x.ServiceType == typeof(IDatabaseMode));
                //Remove it
                context.Remove(existingServiceDefinition);
                //Register DatabaseModeService with implementation as service type.
                context.AddSingleton(existingServiceDefinition.ImplementationType);
                //Registera a IDatabaseMode service with SwitchableDatabaseMode resolving DatabaseModeService as inner
                context.AddSingleton<IDatabaseMode>((sp) => new SwitchableDatabaseMode(sp.GetService(existingServiceDefinition.ImplementationType) as IDatabaseMode));

                context.AddSingleton<IModuleProvider, FakeModuleProvider>();
             });

            if (_customRegistrations != null)
            {
                builder.ConfigureServices(_customRegistrations);
            }
        }
    }
}
