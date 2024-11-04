using System.Linq;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews;

internal class ReviewLocationPreviewPluginController : Controller
{
    private readonly IApprovalReviewsRepository _repository;
    private readonly ReviewUrlGenerator _reviewUrlGenerator;

    public ReviewLocationPreviewPluginController(IApprovalReviewsRepository repository, ReviewUrlGenerator reviewUrlGenerator)
    {
        _repository = repository;
        _reviewUrlGenerator = reviewUrlGenerator;
    }

    public IActionResult Index()
    {
        var viewModel = new ViewModel
        {
            ControllerUrl = _reviewUrlGenerator.ReviewLocationPluginUrl
        };

        return View("Index", viewModel);
    }

    public ActionResult GetAll()
    {
        var result = _repository.LoadAll().GroupBy(x => x.ContentLink.ToReferenceWithoutVersion()).Select(x => new
        {
            Id = x.Key,
            ContentLinks = x.Select(c => new { c.ContentLink, c.SerializedReview })
        });

        return new RestResult { Data = result };
    }

    [HttpPost]
    public void DeleteReviewLocation([FromBody] Dto dto)
    {
        _repository.Delete(ContentReference.Parse(dto.ContentLink));
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
