using System.Web.Mvc;
using System.Web.Routing;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews
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

            RegisterEditPageGet(e, externalReviewOptions);
            RegisterAddPinPost(e.Routes, externalReviewOptions);
        }

        private static void RegisterEditPageGet(RouteRegistrationEventArgs e, ExternalReviewOptions externalReviewOptions)
        {
            var routeValues = new RouteValueDictionary();
            routeValues.Add("controller", "PagePreview");
            routeValues.Add("action", "Index");
            routeValues.Add("token", " UrlParameter.Optional");

            var route = new Route(externalReviewOptions.ReviewsUrl + "/{token}", routeValues, new MvcRouteHandler());
            string[] allowedMethods = {"GET"};
            var methodConstraints = new HttpMethodConstraint(allowedMethods);
            route.Constraints = new RouteValueDictionary {{"httpMethod", methodConstraints}};

            e.Routes.Add(route);
        }

        private void RegisterAddPinPost(RouteCollection routeCollection, ExternalReviewOptions externalReviewOptions)
        {
            var routeValues = new RouteValueDictionary();
            routeValues.Add("controller", "PagePreview");
            routeValues.Add("action", "AddPin");

            var route = new Route(externalReviewOptions.ReviewsUrl + "/AddPin", routeValues, new MvcRouteHandler());
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
