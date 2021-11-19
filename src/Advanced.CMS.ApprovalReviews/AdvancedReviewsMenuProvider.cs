using System.Collections.Generic;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.ApprovalReviews
{
    [MenuProvider]
    public class AdvancedReviewsMenuProvider : IMenuProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdvancedReviewsMenuProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            //TODO: NETCORE: !minor! ??? var controllerPath = _httpContextAccessor.HttpContext.GetControllerPath(typeof(ReviewLocationPreviewPluginController), "Index");
            // Paths.ToResource works but we should be able to get the controller path
            var controllerPath = Paths.ToResource("advanced-cms-approval-reviews" ,"ReviewLocationPreviewPlugin/Index");
            var urlMenuItem1 = new UrlMenuItem("Advanced approval review", "/global/cms/admin/csp",
                controllerPath)
            {
                Alignment = MenuItemAlignment.Left,
                IsAvailable = _ => true,
                SortIndex = 100
            };

            return new List<MenuItem>(1)
            {
                urlMenuItem1
            };
        }
    }
}
