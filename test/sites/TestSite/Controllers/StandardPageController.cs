using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TestSite.Models;

namespace TestSite.Controllers
{
    public class StandardPageController : PageController<StandardPage>
    {
        public IActionResult Index(StandardPage currentPage)
        {
            return View(currentPage);
        }
    }
}
