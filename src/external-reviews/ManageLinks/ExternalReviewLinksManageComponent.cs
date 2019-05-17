using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace AdvancedExternalReviews.ManageLinks
{
    /// <summary>
    /// Edit Mode component used to manage list of external review links
    /// </summary>
    [Component]
    public class ExternalReviewLinksManageComponent : ComponentDefinitionBase
    {
        public ExternalReviewLinksManageComponent()
            : base("alloy-external-review/external-review-manage-links-component")
        {
            Description = "Manage list of external review links";
            Title = "External review links";

            Categories = new[] {"content"};
            SortOrder = 1000;
            PlugInAreas = new[]
            {
                PlugInArea.Navigation
            };

            Settings.Add(new Setting("initialEditMailMessage", "EDIT"));
            Settings.Add(new Setting("initialViewMailMessage", "VIEW"));
        }
    }
}
