﻿using EPiServer.ServiceLocation;
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
            IsAvailableForUserSelection = options.IsEnabled;
            LanguagePath = "/externalreviews/component";
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
                base.Settings["pinCodeSecurityEnabled"] = _options.PinCodeSecurity.Enabled;
                base.Settings["pinCodeSecurityRequired"] = _options.PinCodeSecurity.Required;
                base.Settings["pinCodeLength"] = _options.PinCodeSecurity.CodeLength;
                base.Settings["isEnabled"] = _options.IsEnabled;

                return base.Settings;
            }
        }
    }
}
