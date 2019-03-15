using System.Net;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;

namespace AdvancedApprovalReviews
{
    [RestStore("approvaladvancedreview")]
    public class ApprovalReviewStore: RestControllerBase
    {
        private readonly IApprovalReviewsRepository _approvalReviewsRepository;

        public ApprovalReviewStore(IApprovalReviewsRepository approvalReviewsRepository)
        {
            _approvalReviewsRepository = approvalReviewsRepository;
        }

        [HttpGet]
        public ActionResult Get(ContentReference id)
        {
            return Rest(_approvalReviewsRepository.Load(id));
        }

        [HttpPost]
        public ActionResult Post(PostReviewModel reviewModel)
        {
            if (reviewModel == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _approvalReviewsRepository.Save(reviewModel.ContentLink, reviewModel.SerializedReview);
            return new RestStatusCodeResult(HttpStatusCode.OK);
        }
    }

    public class PostReviewModel
    {
        public ContentReference ContentLink { get; set; }

        public string SerializedReview { get; set; }
    }
}
