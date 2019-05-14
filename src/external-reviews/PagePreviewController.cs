using System.Net;
using System.Web.Mvc;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Modules.Internal;
using EPiServer.Security;
using EPiServer.Shell.Services.Rest;

namespace AdvancedExternalReviews
{
    public class PagePreviewController: Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;

        public PagePreviewController(IContentLoader contentLoader, IExternalReviewLinksRepository externalReviewLinksRepository)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
        }

        public ActionResult Index(string token)
        {
            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return new HttpNotFoundResult("Content not found");
            }

            var content = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);
            if (!content.QueryDistinctAccess(AccessLevel.Read))
            {
                return new RestStatusCodeResult(HttpStatusCode.Forbidden, "Access denied");
            }

            const string url = "Views/PagePreview/Index.cshtml";

            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PagePreviewController).Assembly, url,
                out var resolvedPath))
            {
                var pagePreviewModel = new ContentPreviewModel
                {
                    Token = token,
                    Name = content.Name,
                    ReviewJsScriptPath = GetJsScriptPath(),
                    ResetCssPath = GetResetCssPath()
                };
                return View(resolvedPath, pagePreviewModel);
            }

            return new HttpNotFoundResult("Content not found");
        }

        [HttpPost]
        public ActionResult AddPin(AddPinModel pinModel)
        {
            //TODO: add points

            return new RestResult()
            {
                Data = new
                {
                    Id = 10
                }
            };
        }

        private string GetJsScriptPath()
        {
            const string url = "Views/external-review-component.js";
            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PagePreviewController).Assembly, url,
                out var jsScriptPath))
            {
                return jsScriptPath;
            }

            return "";
        }

        private string GetResetCssPath()
        {
            const string url = "Views/reset.css";
            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PagePreviewController).Assembly, url,
                out var resetCssPath))
            {
                return resetCssPath;
            }

            return "";
        }
    }

    public class AddPinModel
    {
        public string Message { get; set; }
        public string Priority { get; set; }
    }

    public class ContentPreviewModel
    {
        public string Token { get; set; }
        public string Name { get; set; }

        public string ReviewJsScriptPath { get; set; }
        public string ResetCssPath { get; set; }
    }
}
