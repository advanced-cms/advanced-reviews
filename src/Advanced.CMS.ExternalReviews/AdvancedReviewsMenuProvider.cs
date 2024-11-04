using System.Collections.Generic;
using System.Linq;
using Advanced.CMS.ApprovalReviews;
using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace Advanced.CMS.ExternalReviews;

[MenuProvider]
internal class AdvancedReviewsMenuProvider : IMenuProvider
{
    private readonly ExternalReviewOptions _externalReviewOptions;
    private readonly ReviewUrlGenerator _reviewUrlGenerator;

    public AdvancedReviewsMenuProvider(ExternalReviewOptions externalReviewOptions, ReviewUrlGenerator reviewUrlGenerator)
    {
        _externalReviewOptions = externalReviewOptions;
        _reviewUrlGenerator = reviewUrlGenerator;
    }

    public IEnumerable<MenuItem> GetMenuItems()
    {
        if (!_externalReviewOptions.IsEnabled || !_externalReviewOptions.IsAdminModePinReviewerPluginEnabled)
        {
            return Enumerable.Empty<MenuItem>();
        }

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
