using System;
using System.Net;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell.Services.Rest;

namespace AdvancedApprovalReviews
{
    [RestStore("approvaladvancedreview")]
    public class ApprovalReviewStore : RestControllerBase
    {
        private readonly IContentLoader _contentLoader;
        private readonly IApprovalReviewsRepository _approvalReviewsRepository;

        public ApprovalReviewStore(IContentLoader contentLoader, IApprovalReviewsRepository approvalReviewsRepository)
        {
            _contentLoader = contentLoader;
            _approvalReviewsRepository = approvalReviewsRepository;
        }

        public ActionResult Get(ContentReference id)
        {
            var errorResult = ValidateContent(id);
            if (errorResult != null)
            {
                return errorResult;
            }

            return Rest(_approvalReviewsRepository.Load(id));
        }

        public ActionResult Delete(string id, ContentReference contentLink)
        {
            _approvalReviewsRepository.RemoveReviewLocation(id, contentLink);
            return new EmptyResult();
        }

        public ActionResult Post(PostReviewModel reviewModel)
        {
            if (reviewModel == null || reviewModel.ContentLink == null || reviewModel.ReviewLocation == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var errorResult = ValidateContent(reviewModel.ContentLink);
            if (errorResult != null)
            {
                return errorResult;
            }

            try
            {
                var result = _approvalReviewsRepository.Update(reviewModel.ContentLink, reviewModel.ReviewLocation);
                return Rest(result);
            }
            catch (ReviewLocationNotFoundException)
            {
                return new RestStatusCodeResult(HttpStatusCode.NotFound);
            }
        }

        private ActionResult ValidateContent(ContentReference id)
        {
            if (!_contentLoader.TryGet(id, out IContent content))
            {
                return new HttpNotFoundResult("Content not found");
            }

            if (!content.QueryDistinctAccess(AccessLevel.Edit))
            {
                return new RestStatusCodeResult(HttpStatusCode.Forbidden, "Access denied");
            }

            return null;
        }
    }

    public class PostReviewModel
    {
        public ContentReference ContentLink { get; set; }
        public ReviewLocation ReviewLocation { get; set; }
    }
}
