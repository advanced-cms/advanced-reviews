using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using AdvancedExternalReviews.ReviewLinksRepository;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace AdvancedExternalReviews.PinCodeSecurity
{
    /// <summary>
    /// Service responsible for handling external links PIN code security
    /// </summary>
    public interface IExternalLinkPinCodeSecurityHandler
    {
        /// <summary>
        /// Checks if current user has access to <paramref name="externalReviewLink"/> 
        /// </summary>
        /// <param name="externalReviewLink">External reviews link</param>
        /// <returns></returns>
        bool UserHasAccessToLink(ExternalReviewLink externalReviewLink);

        /// <summary>
        /// Redirect to PIN code login page
        /// </summary>
        /// <param name="externalReviewLink"></param>
        void RedirectToLoginPage(ExternalReviewLink externalReviewLink);

        /// <summary>
        /// Tries to sign in user to allow access to <paramref name="externalReviewLink"/> 
        /// </summary>
        /// <param name="externalReviewLink">External review link</param>
        /// <param name="requestedPinCode">PIN code provide by user on ligin page</param>
        /// <returns>True when PIN code is valid and user was signed in</returns>
        bool TryToSignIn(ExternalReviewLink externalReviewLink, string requestedPinCode);
    }

    [ServiceConfiguration(typeof(IExternalLinkPinCodeSecurityHandler))]
    internal class DefaultExternalLinkPinCodeSecurityHandler : IExternalLinkPinCodeSecurityHandler
    {
        private readonly ExternalReviewOptions _externalReviewOptions;

        public DefaultExternalLinkPinCodeSecurityHandler(ExternalReviewOptions externalReviewOptions)
        {
            _externalReviewOptions = externalReviewOptions;
        }

        public bool UserHasAccessToLink(ExternalReviewLink externalReviewLink)
        {
            // check if PIN code option is enabled
            if (!_externalReviewOptions.PinCodeSecurity.Enabled)
            {
                return true;
            }

            // check if PIN code is required for the link
            if (string.IsNullOrEmpty(externalReviewLink.PinCode))
            {
                return true;
            }

            var user = HttpContext.Current.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                // check if user is in role that allow to access link without PIN code
                if (_externalReviewOptions.PinCodeSecurity.RolesWithoutPin != null)
                {
                    foreach (var role in _externalReviewOptions.PinCodeSecurity.RolesWithoutPin)
                    {
                        if (user.IsInRole(role))
                        {
                            return true;
                        }
                    }
                }

                // check if user is allowed to view the page
                if (user.Identity is ClaimsIdentity identity)
                {
                    var claim = identity.FindFirst("ExternalReviewTokens");
                    if (claim != null)
                    {
                        if (!string.IsNullOrEmpty(claim.Value))
                        {
                            var tokens = claim.Value.Split('|');
                            return tokens.Contains(externalReviewLink.Token);
                        }
                    }
                }
            }

            return false;
        }

        public void RedirectToLoginPage(ExternalReviewLink externalReviewLink)
        {
            HttpContext.Current.Response.Redirect("/" + _externalReviewOptions.PinCodeSecurity.ExternalReviewLoginUrl + "?id=" + externalReviewLink.Token);
        }

        public bool TryToSignIn(ExternalReviewLink externalReviewLink, string requestedPinCode)
        {
            // check if PIN code provided by user match link PIN code
            
            var hash = PinCodeHashGenerator.Hash(requestedPinCode, externalReviewLink.Token);
            if (externalReviewLink.PinCode != hash)
            {
                return false;
            }

            var user = HttpContext.Current.User;

            // user is already authenticated. We need to add him new claims with access to link
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
                    tokens.Add(externalReviewLink.Token);
                    identity.RemoveClaim(claim);
                    identity.AddClaim(new Claim("ExternalReviewTokens", string.Join("|", tokens)));
                }
                else
                {
                    identity.AddClaim(new Claim("ExternalReviewTokens", externalReviewLink.Token));
                }
                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
            }
            else
            {
                // user is not authenticated, we need to authenticate and add claims to link

                var userName = DateTime.Now.ToString("yyyyMMddmmhhss");

                var claims = new List<Claim>();

                // create required claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
                claims.Add(new Claim(ClaimTypes.Name, userName));

                // custom – my serialized AppUserState object
                claims.Add(new Claim("ExternalReviewTokens", externalReviewLink.Token));

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

            return true;
        }
    }
}
