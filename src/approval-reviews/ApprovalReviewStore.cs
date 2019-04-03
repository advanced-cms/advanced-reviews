using System.Net;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;

namespace AdvancedApprovalReviews
{
    [RestStore("approvaladvancedreview")]
    public class ApprovalReviewStore : RestControllerBase
    {
        private readonly IApprovalReviewsRepository _approvalReviewsRepository;
        private readonly object _lock = new object();

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
            if (reviewModel == null || reviewModel.ContentLink == null || reviewModel.ReviewLocation == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
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
    }

    public class PostReviewModel
    {
        public ContentReference ContentLink { get; set; }
        public ReviewLocation ReviewLocation { get; set; }
    }
}
