using System.IO;
using System.Text.Json;
using Advanced.CMS.ExternalReviews;
using Alloy.Sample.Extensions;
using Alloy.Sample.Infrastructure;
using EPiServer.Cms.Shell;
using EPiServer.Cms.TinyMce;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Data;
using EPiServer.Formatters;
using EPiServer.Framework.Web.Resources;
using EPiServer.Scheduler;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Alloy.Sample
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data\\Alloy.mdf");
            var connectionstring = _configuration.GetConnectionString("EPiServerDB") ?? $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={dbPath};Initial Catalog=alloy_mvc_netcore;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";

            services.Configure<SchedulerOptions>(o =>
            {
                o.Enabled = false;
            });

            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionstring);
            });

            services.AddCmsAspNetIdentity<ApplicationUser>();

            if (_webHostingEnvironment.IsDevelopment())
            {
                services.AddUIMappedFileProviders(_webHostingEnvironment.ContentRootPath, @"..\..\");
                services.Configure<ClientResourceOptions>(uiOptions =>
                {
                    uiOptions.Debug = true;
                });
            }

            services.AddMvc();
            services.AddAlloy();
            services.AddCmsHost()
                .AddCmsHtmlHelpers()
                .AddCmsUI()
                .AddAdmin()
                .AddVisitorGroupsUI()
                .AddTinyMce();

            services.AddEmbeddedLocalization<Startup>();

            services.AddAdvancedReviews();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<AdministratorRegistrationPageMiddleware>();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapControllerRoute("Register", "/Register", new { controller = "Register", action = "Index" });
                endpoints.MapRazorPages();
            });
        }
    }
}
