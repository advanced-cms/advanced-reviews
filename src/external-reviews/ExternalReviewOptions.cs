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
    }
}
