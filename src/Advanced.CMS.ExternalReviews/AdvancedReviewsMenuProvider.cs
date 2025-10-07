using Advanced.CMS.ApprovalReviews;
using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace Advanced.CMS.ExternalReviews;

[MenuProvider]
internal class AdvancedReviewsMenuProvider(
    ExternalReviewOptions externalReviewOptions,
    ReviewUrlGenerator reviewUrlGenerator)
    : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        if (!externalReviewOptions.IsEnabled || !externalReviewOptions.IsAdminModePinReviewerPluginEnabled)
        {
            return Enumerable.Empty<MenuItem>();
        }

        var controllerPath = $"{reviewUrlGenerator.ReviewLocationPluginUrl}/Index";

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
