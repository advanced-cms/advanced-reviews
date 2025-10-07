using EPiServer.Personalization.VisitorGroups;
using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;
using Microsoft.Extensions.Options;

namespace Advanced.CMS.ExternalReviews.ManageLinks;

/// <summary>
/// Edit Mode component used to manage list of external review links
/// </summary>
internal class ExternalReviewLinksManageComponent : ComponentDefinitionBase
{
    private readonly ExternalReviewOptions _options;
    private readonly IVisitorGroupRepository _visitorGroupRepository;

    public ExternalReviewLinksManageComponent(IOptions<ExternalReviewOptions> options, IVisitorGroupRepository visitorGroupRepository)
        : base("advanced-cms-approval-reviews/dist/external-review-manage-links-widget")
    {
        _options = options.Value;
        _visitorGroupRepository = visitorGroupRepository;
        IsAvailableForUserSelection = _options.IsEnabled;
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
            base.Settings["availableVisitorGroups"] = _visitorGroupRepository.List();
            base.Settings["pinCodeLength"] = _options.PinCodeSecurity.CodeLength;
            base.Settings["isEnabled"] = _options.IsEnabled;
            base.Settings["prolongDays"] = _options.ProlongDays;

            return base.Settings;
        }
    }
}
