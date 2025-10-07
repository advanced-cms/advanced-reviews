using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews;

internal class ReviewLocationPreviewPluginController(
    IApprovalReviewsRepository repository,
    ReviewUrlGenerator reviewUrlGenerator)
    : Controller
{
    public IActionResult Index()
    {
        var viewModel = new ViewModel
        {
            ControllerUrl = reviewUrlGenerator.ReviewLocationPluginUrl
        };

        return View("Index", viewModel);
    }

    public ActionResult GetAll()
    {
        var result = repository.LoadAll().GroupBy(x => x.ContentLink.ToReferenceWithoutVersion()).Select(x => new
        {
            Id = x.Key,
            ContentLinks = x.Select(c => new { c.ContentLink, c.SerializedReview })
        });

        return new RestResult { Data = result };
    }

    [HttpPost]
    public void DeleteReviewLocation([FromBody] Dto dto)
    {
        repository.Delete(ContentReference.Parse(dto.ContentLink));
    }
}

internal class Dto
{
    public string ContentLink { get; set; }
}

public class ViewModel
{
    public string ControllerUrl { get; set; }
}
