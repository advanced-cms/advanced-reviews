using System.Web.Mvc;
using System.Web.Routing;
using AdvancedExternalReviews.PinCodeSecurity;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews.EditReview
{
    [InitializableModule]
    public class GlobalRouteInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        public void Initialize(InitializationEngine context)
        {
            Global.RoutesRegistrating += Global_RoutesRegistrating;
        }

        public void Uninitialize(InitializationEngine context)
        {
            Global.RoutesRegistrating -= Global_RoutesRegistrating;
        }

        private void Global_RoutesRegistrating(object sender, EPiServer.Web.Routing.RouteRegistrationEventArgs e)
        {
            var externalReviewOptions = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            if (!externalReviewOptions.IsEnabled)
            {
                return;
            }

            GlobalFilters.Filters.Add(new ConvertEditLinksFilter());

            RegisterLoginPage(e.Routes, externalReviewOptions);
            RegisterSubmitLoginPage(e.Routes, externalReviewOptions);
        }

        private void RegisterLoginPage(RouteCollection routeCollection, ExternalReviewOptions options)
        {
            if (!options.PinCodeSecurity.Enabled)
            {
                return;
            }
            var routeValues = new RouteValueDictionary();
            routeValues.Add("controller", options.PinCodeSecurity.ExternalReviewLoginUrl);
            routeValues.Add("action", nameof(ExternalReviewLoginController.Index));

            var route = new Route("ExternalReviewLogin", routeValues, new MvcRouteHandler());
            string[] allowedMethods = { "GET" };
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary { { "httpMethod", methodConstraints } };

            routeCollection.Add(route);
        }

        private void RegisterSubmitLoginPage(RouteCollection routeCollection, ExternalReviewOptions options)
        {
            if (!options.PinCodeSecurity.Enabled)
            {
                return;
            }
            var routeValues = new RouteValueDictionary();
            routeValues.Add("controller", options.PinCodeSecurity.ExternalReviewLoginUrl);
            routeValues.Add("action", nameof(ExternalReviewLoginController.Submit));

            var route = new Route("ExternalReviewLogin/Submit", routeValues, new MvcRouteHandler());
            string[] allowedMethods = { "POST" };
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary { { "httpMethod", methodConstraints } };

            routeCollection.Add(route);
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
