using AdvancedExternalReviews.Properties;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    [Options]
    public class ExternalReviewOptions
    {
        public string ReviewsUrl { get; set; } = "externalContentReviews";

        /// <summary>
        /// URL used for displaying readonly version of page
        /// </summary>
        public string ContentPreviewUrl { get; set; } = "externalContentView";

        /// <summary>
        /// email template used for editable links
        /// </summary>
        public string EmailEdit { get; set; } = Resources.mail_edit;

        /// <summary>
        /// email template used for readonly content links
        /// </summary>
        public string EmailView { get; set; } = Resources.mail_preview;
    }
}
