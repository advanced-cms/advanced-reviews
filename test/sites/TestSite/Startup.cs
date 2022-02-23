using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.Admin;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Cms.UI.VisitorGroups;
using EPiServer.Data;
using EPiServer.Framework.Web.Resources;
using EPiServer.Scheduler;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Advanced.CMS.AdvancedReviews;
using Advanced.CMS.IntegrationTests;

namespace TestSite
{
    public class Startup
    {
        public static string DatabaseName { get; set; }
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data\\cms.mdf");
            var connectionString = Configuration.GetConnectionString("EPiServerDB") ?? $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={dbPath};Initial Catalog={DatabaseName};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionString);
            });
            services.Configure<ClientResourceOptions>(o => o.Debug = true);

            //NETCORE: Skip Antiforgery checks in tests since we are constructing test requests programatically out of browser
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IFilterProvider, SkipAntiforgeryFilterProvider>());
            services.Configure<SchedulerOptions>(options =>
            {
                options.Enabled = false;
                options.PingTime = new TimeSpan(10, 10, 10);
            });

            //NETCORE: Consider add appsettings support for this
            // services.AddUIMappedFileProviders(_webHostingEnvironment.ContentRootPath, @"..\..\..\");

            var builder = services.AddMvc();

            services.AddStartupFilter<AssignUser>();
            services.AddCmsHost()
               .AddCmsHtmlHelpers()
               .AddCmsUI()
               .AddAdmin()
               .AddVisitorGroupsUI()
               .AddCmsAspNetIdentity<ApplicationUser>();

            services.AddAdvancedReviews();

            services.Configure<StaticFileOptions>("foo", o => o.OnPrepareResponse = c => c.Context.Response.Headers.Add("X-From-Custom-Option", "Something"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<FakeUserMiddleware>();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Do some registration before MapContent
                endpoints.MapDefaultControllerRoute();
                endpoints.MapContent();
                //and some other here since we want to have test that ensure that both pre and post registration works
                endpoints.MapControllerRoute("additional", "/another/way/to/say/hello", new { controller = "Partner", action = "Hello" });
                endpoints.MapControllerRoute("allproducts", "/partner/allproducts", new { controller = "Partner", action = "GetAllProducts" });
                endpoints.MapControllerRoute("quicknavigator", "/partner/quicknavigator", new { controller = "Partner", action = "QuickNavigator" });

            });
        }
        public class AssignUser : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextAction)
            {
                return app =>
                {
                    app.Use(async (context, next) =>
                    {
                        await AssignUserToContext(context.Request.HttpContext);
                        await next();
                    });
                    nextAction(app);
                };
            }

            private async Task AssignUserToContext(HttpContext context)
            {
                var userName = context.Request.Query["user"];
                if (string.IsNullOrEmpty(userName)) return;

                var pwd = context.Request.Query["pwd"];
                var aum = context.RequestServices.GetService<ApplicationUserProvider<ApplicationUser>>();
                var u = await aum.GetUserAsync(userName);
                var sm = context.RequestServices.GetService<ApplicationSignInManager<ApplicationUser>>();
                var pwdRes = await sm.CheckPasswordSignInAsync(u as ApplicationUser, pwd, false);
                if (pwdRes.Succeeded)
                {
                    var res = await sm.GenerateUserIdentityAsync(u as ApplicationUser);
                    var cp = new ClaimsPrincipal(res);
                    context.User = cp;
                }
            }
        }
    }
}
