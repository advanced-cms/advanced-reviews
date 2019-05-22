using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdvancedApprovalReviews;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Modules.Internal;
using EPiServer.Shell.Services.Rest;

namespace AdvancedExternalReviews
{
    /// <summary>
    /// Controller used to render editable external review page
    /// </summary>
    public class PagePreviewController: Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly IApprovalReviewsRepository _approvalReviewsRepository;

        public PagePreviewController(IContentLoader contentLoader,
            IExternalReviewLinksRepository externalReviewLinksRepository,
            IApprovalReviewsRepository approvalReviewsRepository)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _approvalReviewsRepository = approvalReviewsRepository;
        }

        public ActionResult Index(string token)
        {
            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return new HttpNotFoundResult("Content not found");
            }

            var content = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);
            /*if (!content.QueryDistinctAccess(AccessLevel.Read))
            {
                return new RestStatusCodeResult(HttpStatusCode.Forbidden, "Access denied");
            }*/

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
        public ActionResult AddPin(ReviewLocation reviewLocation)
        {
            // get token based on URL segment
            string GetToken()
            {
                var request = System.Web.HttpContext.Current.Request;
                if (request.UrlReferrer == null)
                {
                    return null;
                }

                var segements = request.UrlReferrer.Segments;
                if (segements.Length == 0)
                {
                    return null;
                }

                var lastSegment = segements.Last();
                return lastSegment;
            }

            var token = GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (reviewLink == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO: verify number of items added with token. There should be max size

            //TODO: security issue - we post whole item and external reviewer can modify this

            var location = _approvalReviewsRepository.Update(reviewLink.ContentLink, reviewLocation);

            return new RestResult
            {
                Data = location
            };
        }

        private static string GetJsScriptPath()
        {
            const string url = "Views/external-review-component.js";
            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PagePreviewController).Assembly, url,
                out var jsScriptPath))
            {
                return jsScriptPath;
            }

            return "";
        }

        private static string GetResetCssPath()
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

    public class ContentPreviewModel
    {
        public string Token { get; set; }
        public string Name { get; set; }

        public string ReviewJsScriptPath { get; set; }
        public string ResetCssPath { get; set; }
    }
}
