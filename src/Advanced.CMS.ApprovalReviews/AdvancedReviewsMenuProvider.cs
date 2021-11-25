using System.Collections.Generic;
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
            var urlMenuItem1 = new UrlMenuItem("Advanced approval review", "/global/cms/admin/reviewplugin",
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
