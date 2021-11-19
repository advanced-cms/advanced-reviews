using System;
using System.Linq;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.ExternalReviews
{
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

            return services;
        }
    }
}
