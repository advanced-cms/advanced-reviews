using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Framework.Modules.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedExternalReviews.PinCodeSecurity
{
    /// <summary>
    /// Controller used to authenticate user using PIN code
    /// </summary>
    public class ExternalReviewLoginController : Controller
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExternalReviewLoginController(IExternalReviewLinksRepository externalReviewLinksRepository, IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler, IHttpContextAccessor httpContextAccessor)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var token = _httpContextAccessor.HttpContext.Request.RouteValues["id"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                return new NotFoundObjectResult("Content not found");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
            {
                return new NotFoundObjectResult("Content not found");
            }

            const string url = "Views/ExternalReviewLogin/Index.cshtml";

            if (ModuleResourceResolver.Instance.TryResolvePath(typeof(ExternalReviewLoginController).Assembly, url,
                out var resolvedPath))
            {
                return View(resolvedPath);
            }
            return new NotFoundObjectResult("Page not found");
        }

        [HttpPost]
        public ActionResult Submit(LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Token))
            {
                return new NotFoundObjectResult("Content not found");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(loginModel.Token);
            if (string.IsNullOrEmpty(externalReviewLink?.PinCode))
            {
                return new NotFoundObjectResult("Content not found");
            }

            if (_externalLinkPinCodeSecurityHandler.TryToSignIn(externalReviewLink, loginModel.Code))
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
}
