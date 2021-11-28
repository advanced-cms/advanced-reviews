using System.Collections.Generic;
using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace Advanced.CMS.ApprovalReviews
{
    [MenuProvider]
    public class AdvancedReviewsMenuProvider : IMenuProvider
    {
        private readonly ReviewUrlGenerator _reviewUrlGenerator;

        public AdvancedReviewsMenuProvider(ReviewUrlGenerator reviewUrlGenerator)
        {
            _reviewUrlGenerator = reviewUrlGenerator;
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            var controllerPath = $"{_reviewUrlGenerator.ReviewLocationPluginUrl}/Index";

            return new List<MenuItem>
            {
                new UrlMenuItem("Advanced approval review", MenuPaths.Global + "/cms/admin/reviewsplugin",
                    controllerPath)
                {
                    Alignment = MenuItemAlignment.Left,
                    SortIndex = SortIndex.Last,
                    AuthorizationPolicy = CmsPolicyNames.CmsAdmin
                }
            };
        }
    }
}
