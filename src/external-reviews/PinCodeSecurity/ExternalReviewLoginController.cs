using System.Web.Mvc;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Framework.Modules.Internal;

namespace AdvancedExternalReviews.PinCodeSecurity
{
    /// <summary>
    /// Controller used to authenticate user using PIN code
    /// </summary>
    public class ExternalReviewLoginController : Controller
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;

        public ExternalReviewLoginController(IExternalReviewLinksRepository externalReviewLinksRepository, IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var token = System.Web.HttpContext.Current.Request["id"];

            if (string.IsNullOrEmpty(token))
            {
                return new HttpNotFoundResult("Content not found");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
            {
                return new HttpNotFoundResult("Content not found");
            }

            const string url = "Views/ExternalReviewLogin/Index.cshtml";

            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(ExternalReviewLoginController).Assembly, url,
                out var resolvedPath))
            {
                return View(resolvedPath);
            }
            return new HttpNotFoundResult("Page not found");
        }

        [HttpPost]
        public ActionResult Submit(LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Token))
            {
                return new HttpNotFoundResult("Content not found");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(loginModel.Token);
            if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
            {
                return new HttpNotFoundResult("Content not found");
            }

            if (_externalLinkPinCodeSecurityHandler.TryToSignIn(externalReviewLink, loginModel.Code))
            {
                return new RedirectResult(externalReviewLink.LinkUrl);
            }
            else
            {
                return new HttpNotFoundResult("Content not found");
            }
        }
    }

    public class LoginModel
    {
        public string Code { get; set; }
        public string Token { get; set; }
    }
}
