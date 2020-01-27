using System;
using System.Collections.Generic;
using AdvancedExternalReviews.Properties;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace AdvancedExternalReviews
{
    [Options]
    public class ExternalReviewOptions
    {
        public string ReviewsUrl { get; set; } = "externalContentReviews";

        /// <summary>
        /// Gets or sets if the plugin should be initialized in Edit Mode
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// URL used for displaying readonly version of page
        /// </summary>
        public string ContentPreviewUrl { get; set; } = "externalContentView";

        /// <summary>
        /// URL to editable content iframe
        /// </summary>
        public string ContentIframeEditUrlSegment { get; set; } = "externalPageReview";

        /// <summary>
        /// email subject template
        /// </summary>
        public string EmailSubject { get; set; } = Resources.EmailSubject;

        /// <summary>
        /// email template used for editable links
        /// </summary>
        public string EmailEdit { get; set; } = Resources.mail_edit;

        /// <summary>
        /// email template used for readonly content links
        /// </summary>
        public string EmailView { get; set; } = Resources.mail_preview;

        /// <summary>
        /// When true then Editor can create editable links
        /// </summary>
        public bool EditableLinksEnabled { get; set; } = false;

        /// <summary>
        /// For how long view link is valid
        /// </summary>
        public TimeSpan ViewLinkValidTo { get; set; } = TimeSpan.FromDays(5);

        /// <summary>
        /// For how long editable link is valid
        /// </summary>
        public TimeSpan EditLinkValidTo { get; set; } = TimeSpan.FromDays(5);

        /// <summary>
        /// Restriction options
        /// </summary>
        public ExternalReviewRestrictionOptions Restrictions { get; } = new ExternalReviewRestrictionOptions();

        public PinCodeSecurityOptions PinCodeSecurity { get; } = new PinCodeSecurityOptions();
    }

    public class ExternalReviewRestrictionOptions
    {
        /// <summary>
        /// Maximum number of review locations that can be added to the page
        /// </summary>
        public int MaxReviewLocationsForContent { get; set; } = int.MaxValue;

        /// <summary>
        /// Maximum number of comments that can be added to one review location
        /// </summary>
        public int MaxCommentsForReviewLocation { get; set; } = int.MaxValue;

        /// <summary>
        /// maximum length of comment
        /// </summary>
        public int MaxCommentLength { get; set; } = int.MaxValue;
    }

    public class PinCodeSecurityOptions
    {
        /// <summary>
        /// When true, then PIN code check is enabled
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// URL for login page
        /// </summary>
        public string ExternalReviewLoginUrl { get; set; } = "ExternalReviewLogin";

        /// <summary>
        /// Roles that can access links without PIN
        /// </summary>
        public IEnumerable<string> RolesWithoutPin { get; set; } = new[] {"WebEditors", "WebAdmins"};

        /// <summary>
        /// For how long authentication cookie should be valid
        /// </summary>
        public TimeSpan AuthenticationCookieLifeTime { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// PIN code length
        /// </summary>
        public int CodeLength { get; set; } = 4;
    }

    public class AdvancedReviewsModule : ShellModule
    {
        public AdvancedReviewsModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        {
        }

        /// <inheritdoc />
        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            return new AdvancedReviewsModuleViewModel(this, clientResourceService, options);
        }
    }

    public class AdvancedApprovalModule : ShellModule
    {
        public AdvancedApprovalModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        {
        }

        /// <inheritdoc />
        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            return new AdvancedReviewsModuleViewModel(this, clientResourceService, options);
        }
    }

    public class AdvancedReviewsModuleViewModel : ModuleViewModel
    {
        public AdvancedReviewsModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService, ExternalReviewOptions options) :
            base(shellModule, clientResourceService)
        {
            Options = options;
        }

        public ExternalReviewOptions Options { get; }
    }
}
