using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdvancedApprovalReviews;
using AdvancedApprovalReviews.Notifications;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Framework.Modules.Internal;
using EPiServer.Framework.Serialization;
using EPiServer.Shell.Services.Rest;

namespace AdvancedExternalReviews.EditReview
{
    /// <summary>
    /// Controller used to render editable external review page
    /// </summary>
    public class PageEditController : Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly IApprovalReviewsRepository _approvalReviewsRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly IObjectSerializerFactory _serializerFactory;
        private readonly IStartPageUrlResolver _startPageUrlResolver;
        private readonly PropertyResolver _propertyResolver;
        private readonly ReviewsNotifier _reviewsNotifier;

        public PageEditController(IContentLoader contentLoader,
            IExternalReviewLinksRepository externalReviewLinksRepository,
            IApprovalReviewsRepository approvalReviewsRepository,
            ExternalReviewOptions externalReviewOptions, IObjectSerializerFactory serializerFactory,
            IStartPageUrlResolver startPageUrlResolver,
            PropertyResolver propertyResolver,
            ReviewsNotifier reviewsNotifier)
        {
            _contentLoader = contentLoader;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _approvalReviewsRepository = approvalReviewsRepository;
            _externalReviewOptions = externalReviewOptions;
            _serializerFactory = serializerFactory;
            _startPageUrlResolver = startPageUrlResolver;
            _propertyResolver = propertyResolver;
            _reviewsNotifier = reviewsNotifier;

            approvalReviewsRepository.OnBeforeUpdate += ApprovalReviewsRepository_OnBeforeUpdate;
        }

        [ConvertEditLinksFilter]
        public ActionResult Index(string token)
        {
            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsEditableLink())
            {
                return new HttpNotFoundResult("Content not found");
            }

            var content = _contentLoader.Get<IContent>(externalReviewLink.ContentLink);

            const string url = "Views/PagePreview/Index.cshtml";
            var startPageUrl = _startPageUrlResolver.GetUrl(externalReviewLink.ContentLink, content.LanguageBranch());

            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PageEditController).Assembly, url,
                out var resolvedPath))
            {
                var serializer = _serializerFactory.GetSerializer(KnownContentTypes.Json);
                var pagePreviewModel = new ContentPreviewModel
                {
                    Token = token,
                    Name = content.Name,
                    EditableContentUrlSegment = UrlPath.Combine(startPageUrl, _externalReviewOptions.ContentIframeEditUrlSegment, token),
                    AddPinUrl = $"{UrlPath.EnsureStartsWithSlash(_externalReviewOptions.ReviewsUrl)}/AddPin",
                    RemovePinUrl = $"{UrlPath.EnsureStartsWithSlash(_externalReviewOptions.ReviewsUrl)}/RemovePin",
                    ReviewJsScriptPath = GetJsScriptPath(),
                    ResetCssPath = GetResetCssPath(),
                    ReviewPins = serializer.Serialize(_approvalReviewsRepository.Load(externalReviewLink.ContentLink)),
                    Metadata = serializer.Serialize(_propertyResolver.Resolve(content as ContentData)),
                    Options = serializer.Serialize(_externalReviewOptions)
                };
                return View(resolvedPath, pagePreviewModel);
            }

            return new HttpNotFoundResult("Content not found");
        }

        [HttpPost]
        public ActionResult AddPin(ReviewLocation reviewLocation)
        {
            var token = reviewLocation.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (reviewLink == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!ValidateReviewLocation(reviewLocation))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO: security issue - we post whole item and external reviewer can modify this

            _reviewsNotifier.NotifyCmsEditor(reviewLink.ContentLink, token, reviewLocation.Data, false);

            var location = _approvalReviewsRepository.Update(reviewLink.ContentLink, reviewLocation);
            if (location == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new RestResult
            {
                Data = location
            };
        }

        [HttpPost]
        public ActionResult RemovePin(DeleteReviewLocation location)
        {
            var token = location.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (reviewLink == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _approvalReviewsRepository.RemoveReviewLocation(location.Id, reviewLink.ContentLink);
            return new EmptyResult();
        }

        private bool ValidateReviewLocation(ReviewLocation reviewLocation)
        {
            bool ValidateComment(CommentDto comment)
            {
                return comment.Text.Length <= _externalReviewOptions.Restrictions.MaxCommentLength;
            }

            var serializer = _serializerFactory.GetSerializer(KnownContentTypes.Json);
            var reviewLocationDto = serializer.Deserialize<ReviewLocationDto>(reviewLocation.Data);
            if (reviewLocationDto == null)
            {
                return false;
            }

            if (!ValidateComment(reviewLocationDto.FirstComment))
            {
                return false;
            }

            if (reviewLocationDto.Comments.Count() > _externalReviewOptions.Restrictions.MaxCommentsForReviewLocation)
            {
                return false;
            }

            foreach (var comment in reviewLocationDto.Comments)
            {
                if (!ValidateComment(comment))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetJsScriptPath()
        {
            const string url = "Views/external-review-component.js";
            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PageEditController).Assembly, url,
                out var jsScriptPath))
            {
                return jsScriptPath;
            }

            return "";
        }

        private static string GetResetCssPath()
        {
            const string url = "Views/reset.css";
            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(PageEditController).Assembly, url,
                out var resetCssPath))
            {
                return resetCssPath;
            }

            return "";
        }

        private void ApprovalReviewsRepository_OnBeforeUpdate(object sender, BeforeUpdateEventArgs e)
        {
            if (e.IsNew == false)
            {
                return;
            }

            if (e.ReviewLocations.Count() > _externalReviewOptions.Restrictions.MaxReviewLocationsForContent)
            {
                e.Cancel = true;
            }
        }

        private class ReviewLocationDto
        {
            public CommentDto FirstComment { get; set; }
            public IEnumerable<CommentDto> Comments { get; set; }
        }

        private class CommentDto
        {
            public string Text { get; set; }
        }
    }

    public class DeleteReviewLocation
    {
        public string Token { get; set; }
        public string Id { get; set; }
    }

    public class ContentPreviewModel
    {
        public string Token { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Url used by the iframe, it contains specific language branch in which the content was created
        /// </summary>
        public string EditableContentUrlSegment { get; set; }

        /// <summary>
        /// Url where new pins will be posted to
        /// </summary>
        public string AddPinUrl { get; set; }
        public string RemovePinUrl { get; set; }

        public string ReviewJsScriptPath { get; set; }
        public string ResetCssPath { get; set; }
        public string ReviewPins { get; set; }
        public string Metadata { get; set; }
        public string Options { get; set; }
    }
}

//TODO: pass restrictions to client?
