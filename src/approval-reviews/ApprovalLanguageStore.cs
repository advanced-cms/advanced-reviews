using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedApprovalReviews
{
    [RestStore("approvallanguage")]
    //TODO: any better way to do this?
    public class ApprovalLanguageStore : RestControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var _username = User.Identity.Name;
            var profile = EPiServerProfile.Get(_username);

            return Rest(profile.Language);
        }
    }
}
