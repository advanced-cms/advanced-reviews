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
        private readonly ExternalReviewOptions _options;

        public ExternalReviewLinksManageComponent() : this(ServiceLocator.Current.GetInstance<ExternalReviewOptions>())
        {
        }

        public ExternalReviewLinksManageComponent(ExternalReviewOptions options)
            : base("advanced-cms-external-review/external-review-manage-links-component")
        {
            _options = options;

            Description = "Manage list of external review links";
            Title = "External review links";

            Categories = new[] {"content"};
            SortOrder = 1000;
            PlugInAreas = new[]
            {
                PlugInArea.Navigation
            };
        }

        public override ISettingsDictionary Settings {
            get
            {
                base.Settings["initialMailSubject"] = _options.EmailSubject;
                base.Settings["initialEditMailMessage"] = _options.EmailEdit;
                base.Settings["initialViewMailMessage"] = _options.EmailView;
                base.Settings["editableLinksEnabled"] = _options.EditableLinksEnabled;

                return base.Settings;
            }
        }
    }
}
