using System.Linq;
using System.Security.Claims;
using System.Web;
using AdvancedExternalReviews.ReviewLinksRepository;

namespace AdvancedExternalReviews.PinCodeSecurity
{
    internal static class ExternalReviewsSecurityExtensions
    {
        public static bool CheckAuthenticated(this ExternalReviewOptions options, ExternalReviewLink externalReviewLink)
        {
            // check if PIN code option is enabled
            if (!options.PinCodeSecurity.Enabled)
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
                if (options.PinCodeSecurity.RolesWithoutPin != null)
                {
                    foreach (var role in options.PinCodeSecurity.RolesWithoutPin)
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

        public static void RedirectToLoginPage(this ExternalReviewOptions externalReviewOptions, ExternalReviewLink externalReviewLink)
        {
            HttpContext.Current.Response.Redirect("/" + externalReviewOptions.PinCodeSecurity.ExternalReviewLoginUrl + "?id=" + externalReviewLink.Token);
        }
    }
}
