using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.Framework.Modules.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace AdvancedExternalReviews.PinCodeSecurity
{
    /// <summary>
    /// Controller used to authenticate user using PIN code
    /// </summary>
    public class ExternalReviewLoginController : Controller
    {
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public ExternalReviewLoginController(IExternalReviewLinksRepository externalReviewLinksRepository, ExternalReviewOptions externalReviewOptions)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var token = System.Web.HttpContext.Current.Request["id"];
            if (!ValidateByToken(token))
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
            if (!ValidateByToken(loginModel.Token))
            {
                return new HttpNotFoundResult("Content not found");
            }
            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(loginModel.Token);
            var hash = PinCodeHashGenerator.Hash(loginModel.Code, externalReviewLink.Token);
            if (externalReviewLink.PinCode != hash)
            {
                return new HttpUnauthorizedResult();
            }

            var user = System.Web.HttpContext.Current.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated && user.Identity is ClaimsIdentity)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var claim = identity.FindFirst("ExternalReviewTokens");
                if (claim != null)
                {
                    var tokens = new List<string>();
                    if (!string.IsNullOrEmpty(claim.Value))
                    {
                        tokens.AddRange(claim.Value.Split('|'));
                    }
                    tokens.Add(loginModel.Token);
                    identity.RemoveClaim(claim);
                    identity.AddClaim(new Claim("ExternalReviewTokens", string.Join("|", tokens)));
                }
                else
                {
                    identity.AddClaim(new Claim("ExternalReviewTokens", loginModel.Token));
                }
                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
            }
            else
            {
                var userName = DateTime.Now.ToString("yyyyMMddmmhhss");

                var claims = new List<Claim>();

                // create required claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
                claims.Add(new Claim(ClaimTypes.Name, userName));

                // custom – my serialized AppUserState object
                claims.Add(new Claim("ExternalReviewTokens", loginModel.Token));

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.Add(_externalReviewOptions.PinCodeSecurity.AuthenticationCookieLifeTime),
                    RedirectUri = externalReviewLink.LinkUrl
                }, identity);
            }

            return new RedirectResult(externalReviewLink.LinkUrl);
        }

        private bool ValidateByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (string.IsNullOrEmpty(externalReviewLink.PinCode))
            {
                return false;
            }

            return true;
        }
    }

    public class LoginModel
    {
        public string Code { get; set; }
        public string Token { get; set; }
    }
}
