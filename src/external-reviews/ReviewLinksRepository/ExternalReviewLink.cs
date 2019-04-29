using System;
using EPiServer.Core;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    public class ExternalReviewLink
    {
        public ContentReference ContentLink { get; set; }
        public bool IsEditable { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public string LinkUrl { get; set; }

        internal static ExternalReviewLink FromExternalReview(ExternalReviewLinkDds externalReviewLinkDds, string editUrl, string previewUrl)
        {
            previewUrl = "en/" + previewUrl; //TODO: #externalreview hardcoded

            return new ExternalReviewLink
            {
                ContentLink = externalReviewLinkDds.ContentLink,
                IsEditable = externalReviewLinkDds.IsEditable,
                Token = externalReviewLinkDds.Token,
                ValidTo = externalReviewLinkDds.ValidTo,
                LinkUrl = "/" +  (externalReviewLinkDds.IsEditable ? editUrl: previewUrl) + "/" + externalReviewLinkDds.Token //TODO: externalReviews URL
            };
        }
    }
}
