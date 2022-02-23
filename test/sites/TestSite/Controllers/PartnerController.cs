using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Shell.Web.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace TestSite.Controllers
{
    public class PartnerController : Controller
    {
        private readonly NavigationService _navigationService;

        public PartnerController(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Hello()
        {
            return Content("World");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = _navigationService.GetAllProductsAsync();
            var productCount = await result.CountAsync();
            if (productCount > 0)
                return Ok(result);
            else
                return NoContent();
        }

        [HttpGet]
        public IActionResult QuickNavigator()
        {
            return View();
        }
    }
}
