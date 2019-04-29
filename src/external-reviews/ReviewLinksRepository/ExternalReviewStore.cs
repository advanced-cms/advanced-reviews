using System.Net;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    [RestStore("externalreviewstore")]
    public class ApprovalReviewStore : RestControllerBase
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly object _lock = new object();

        public ApprovalReviewStore(IExternalReviewLinksRepository externalReviewLinksRepository)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
        }

        [HttpGet]
        public ActionResult Get(ContentReference id)
        {
            return Rest(_externalReviewLinksRepository.GetLinksForContent(id));
        }

        [HttpPost]
        public ActionResult Post(PostExternalReviewLinkModel externalLink)
        {
            if (externalLink == null || externalLink.ContentLink == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = _externalReviewLinksRepository.AddLink(externalLink.ContentLink, externalLink.IsEditable);
            return Rest(result);
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            _externalReviewLinksRepository.DeleteLink(id);
            return Rest(true);
        }
    }

    public class PostExternalReviewLinkModel
    {
        public ContentReference ContentLink { get; set; }
        public bool IsEditable { get; set; }
    }
}
