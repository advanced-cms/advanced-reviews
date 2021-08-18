using System.Linq;
using AlloyTemplates.Controllers;
using AlloyTemplates.Models.Pages;
using AlloyTemplates.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EPiServer.Shell.Search;

namespace AlloyMvcTemplates.Controllers
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
