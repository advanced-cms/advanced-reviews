using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace AdvancedExternalReviews
{
    [Component]
    public class FavouriteContent : ComponentDefinitionBase
    {
        public FavouriteContent()
            : base("alloy-external-review/external-review-component")
        {
            Description = "Manage list of external review links";
            Title = "External review links";

            this.Categories = new[] {"content"};
            this.SortOrder = 1000;
            this.PlugInAreas = new[]
            {
                PlugInArea.Navigation
            };
        }
    }
}
