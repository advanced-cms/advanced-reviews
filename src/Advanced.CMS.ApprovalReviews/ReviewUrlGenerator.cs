using Advanced.CMS.ApprovalReviews.AvatarsService;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Advanced.CMS.ApprovalReviews
{
    [ServiceConfiguration]
    public class ReviewUrlGenerator
    {
        public string AvatarUrl => Paths.ToResource("advanced-cms-approval-reviews", $"ReviewAvatars/{nameof(ReviewAvatarsController.Index)}");
        //TODO: minor, why this does not work??? var controllerPath = _httpContextAccessor.HttpContext.GetControllerPath(typeof(ReviewLocationPreviewPluginController), "Index");
        public string ReviewLocationPluginUrl => Paths.ToResource("advanced-cms-approval-reviews" ,"ReviewLocationPreviewPlugin");
    }
}
