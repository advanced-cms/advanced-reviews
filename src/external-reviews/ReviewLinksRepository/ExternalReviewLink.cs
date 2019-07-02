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
    }
}
