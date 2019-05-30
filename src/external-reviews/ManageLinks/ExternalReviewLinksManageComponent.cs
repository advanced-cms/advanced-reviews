using EPiServer.ServiceLocation;
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
        private readonly ExternalReviewOptions _externalReviewOptions;

        public ExternalReviewLinksManageComponent() : this(ServiceLocator.Current.GetInstance<ExternalReviewOptions>())
        {
        }

        public ExternalReviewLinksManageComponent(ExternalReviewOptions externalReviewOptions)
            : base("alloy-external-review/external-review-manage-links-component")
        {
            _externalReviewOptions = externalReviewOptions;
            Description = "Manage list of external review links";
            Title = "External review links";

            Categories = new[] {"content"};
            SortOrder = 1000;
            PlugInAreas = new[]
            {
                PlugInArea.Navigation
            };

            Settings.Add(new Setting("initialMailSubject", _externalReviewOptions.EmailSubject));
            Settings.Add(new Setting("initialEditMailMessage", _externalReviewOptions.EmailEdit));
            Settings.Add(new Setting("initialViewMailMessage", _externalReviewOptions.EmailView));
        }
    }
}
