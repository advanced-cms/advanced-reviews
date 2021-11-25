using System.Linq;
using Alloy.Sample.Models.Pages;
using Alloy.Sample.Models.ViewModels;
using EPiServer.Shell.Search;
using Microsoft.AspNetCore.Mvc;

namespace Alloy.Sample.Controllers
{
    public class SearchPageController : PageControllerBase<SearchPage>
    {
        private SearchProvidersManager _searchProvidersManager;

        public SearchPageController(SearchProvidersManager searchProvidersManager)
        {
            _searchProvidersManager = searchProvidersManager;
        }

        public ViewResult Index(SearchPage currentPage, string q)
        {
            var providers = _searchProvidersManager.GetEnabledProvidersByPriority("CMS/pages", true);

            var hits = providers.SelectMany(p => p.Search(new Query(q))).ToList();

            var model = new SearchContentModel(currentPage)
            {
                Hits = hits.Select(x => new SearchContentModel.SearchHit
                {
                    Url = x.Url,
                    Excerpt = x.PreviewText,
                    Title = x.Title
                }),
                NumberOfHits = hits.Count,
                SearchServiceDisabled = false,
                SearchedQuery = q
            };

            return View(model);
        }
    }
}
