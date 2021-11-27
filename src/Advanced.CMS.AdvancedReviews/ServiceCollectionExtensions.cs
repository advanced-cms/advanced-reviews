using System;
using System.Linq;
using Advanced.CMS.ExternalReviews;
using EPiServer.Shell.Modules;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace Advanced.CMS.AdvancedReviews
{
    public class AdvancedReviewsEndpointRoutingExtension : IEndpointRoutingExtension
    {
        public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var options = endpointRouteBuilder.ServiceProvider.GetInstance<ExternalReviewOptions>();

            endpointRouteBuilder.MapControllerRoute("ImageProxy", "/ImageProxy",
                new { controller = "ImageProxy", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("ExternalReviewLogin",
                $"/{options.PinCodeSecurity.ExternalReviewLoginUrl}",
                new { controller = "ExternalReviewLogin", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("ExternalReviewLoginSubmit",
                $"/{options.PinCodeSecurity.ExternalReviewLoginUrl}",
                new { controller = "ExternalReviewLogin", action = "Submit" },
                new { httpMethod = new HttpMethodRouteConstraint(HttpMethods.Post) });
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
