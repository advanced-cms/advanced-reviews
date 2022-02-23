using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TestSite.Models;

namespace TestSite.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        public IActionResult Index(StartPage currentPage)
        {
            return View(currentPage);
        }
    }
}
