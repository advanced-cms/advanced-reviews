using System;
using System.Linq;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.ExternalReviews
{
    //TODO NETCORE: why the startup filter from <see cref="Advanced.CMS.ExternalReviews.AdvancedReviewsStartupFilter" />
    public class AdvancedReviewsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                // if (app == null)
                // {
                //     throw new ArgumentNullException(nameof (app));
                // }
                //
                // // var reviewsUrl = app.ApplicationServices.GetInstance<ExternalReviewOptions>().ReviewsUrl;
                // var reviewsUrl = AdvancedReviewsRoutingConstants.ReviewsUrl;
                //
                // app.UseRouting();
                // app.UseEndpoints(endpoints =>
                // {
                //     endpoints.MapControllerRoute(reviewsUrl, $"/{reviewsUrl}/{{token}}",
                //         new { controller = "PageEdit", action = "Index" });
                // });

                next(app);
            };
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdvancedReviews(this IServiceCollection services,
            Action<ExternalReviewOptions> externalReviewOptions = null)
        {
            services.Configure<ProtectedModuleOptions>(
                pm =>
                {
                    if (!pm.Items.Any(i =>
                        i.Name.Equals("advanced-cms-external-reviews", StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails { Name = "advanced-cms-external-reviews" });
                    }

                    if (!pm.Items.Any(i =>
                        i.Name.Equals("advanced-cms-approval-reviews", StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails { Name = "advanced-cms-approval-reviews" });
                    }
                });

            if (externalReviewOptions != null)
            {
                services.Configure(externalReviewOptions);
            }

            // services.AddStartupFilter<AdvancedReviewsStartupFilter>();

            return services;
        }
    }
}
