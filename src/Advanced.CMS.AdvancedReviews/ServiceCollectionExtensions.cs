using System;
using System.Linq;
using Advanced.CMS.ExternalReviews;
using EPiServer.Shell.Modules;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Advanced.CMS.AdvancedReviews
{
    public class AdvancedReviewsEndpointRoutingExtension : IEndpointRoutingExtension
    {
        public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("ImageProxy", "/ImageProxy",
                new { controller = "ImageProxy", action = "Index" });
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

            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof (IEndpointRoutingExtension),
                typeof (AdvancedReviewsEndpointRoutingExtension)));

            return services;
        }
    }
}