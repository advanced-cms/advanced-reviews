using Microsoft.AspNetCore.Mvc;

namespace Alloy.Sample.Controllers
{
    public class RouteAttributeController : ControllerBase
    {
        public const string AttributeRoute = "goto-foo";

        [Route(AttributeRoute)]
        public IActionResult Index() => Content("bar");
    }
}
