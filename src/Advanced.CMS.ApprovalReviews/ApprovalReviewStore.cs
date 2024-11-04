using System.Net;
using Advanced.CMS.ApprovalReviews.Notifications;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews;

[RestStore("approvaladvancedreview")]
internal class ApprovalReviewStore : RestControllerBase
{
    private readonly IContentLoader _contentLoader;
    private readonly IApprovalReviewsRepository _approvalReviewsRepository;
    private readonly ReviewsNotifier _reviewsNotifier;

    public ApprovalReviewStore(IContentLoader contentLoader, IApprovalReviewsRepository approvalReviewsRepository,
        ReviewsNotifier reviewsNotifier)
    {
        _contentLoader = contentLoader;
        _approvalReviewsRepository = approvalReviewsRepository;
        _reviewsNotifier = reviewsNotifier;
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

    [HttpPost]
    public ActionResult Post([FromBody] PostReviewModel reviewModel)
    {
        if (reviewModel?.ContentLink == null || reviewModel.ReviewLocation == null ||
            !ContentReference.TryParse(reviewModel.ContentLink, out var contentLink))
        {
            return new RestStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var errorResult = ValidateContent(contentLink);
        if (errorResult != null)
        {
            return errorResult;
        }

        try
        {
            var result = _approvalReviewsRepository.Update(contentLink, reviewModel.ReviewLocation);
#pragma warning disable 4014
            _reviewsNotifier.NotifyCmsEditor(contentLink, reviewModel.ContentLink, reviewModel.ReviewLocation.Data, true);
#pragma warning restore 4014
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
            return new NotFoundResult();
        }

        if (!content.QueryDistinctAccess(AccessLevel.Edit))
        {
            return new RestStatusCodeResult(HttpStatusCode.Forbidden, "Access denied");
        }

        return null;
    }
}

internal class PostReviewModel
{
    public string ContentLink { get; set; }
    public ReviewLocation ReviewLocation { get; set; }
}
