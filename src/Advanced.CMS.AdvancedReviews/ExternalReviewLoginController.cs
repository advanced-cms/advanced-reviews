using Advanced.CMS.ExternalReviews.PinCodeSecurity;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.AdvancedReviews;

/// <summary>
/// Controller used to authenticate user using PIN code
/// </summary>
internal class ExternalReviewLoginController(
    IExternalReviewLinksRepository externalReviewLinksRepository,
    IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler)
    : Controller
{
    [HttpGet]
    public ActionResult Index(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new NotFoundObjectResult("Content not found");
        }

        var externalReviewLink = externalReviewLinksRepository.GetContentByToken(id);
        if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
        {
            return new NotFoundObjectResult("Content not found");
        }

        return View();
    }

    [HttpPost]
    public ActionResult Submit([FromForm] LoginModel loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Token))
        {
            return new NotFoundObjectResult("Content not found");
        }

        var externalReviewLink = externalReviewLinksRepository.GetContentByToken(loginModel.Token);
        if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
        {
            return new NotFoundObjectResult("Content not found");
        }

        if (externalLinkPinCodeSecurityHandler.TryToSignIn(externalReviewLink, loginModel.Code))
        {
            return new RedirectResult(externalReviewLink.LinkUrl);
        }
        else
        {
            return new NotFoundObjectResult("Content not found");
        }
    }
}

public class LoginModel
{
    public string Code { get; set; }
    public string Token { get; set; }
}
