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

            var content = _contentLoader.Get<PageData>(externalReviewLink.ContentLink);
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
                    Name = content.Name
                };
                return View(resolvedPath, pagePreviewModel);
            }

            return new HttpNotFoundResult("Content not found");
        }
    }

    public class ContentPreviewModel
    {
        public string Token { get; set; }
        public string Name { get; set; }
    }
}
