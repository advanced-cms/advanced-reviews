using System;
using EPiServer.Core;

namespace Advanced.CMS.ExternalReviews.ReviewLinksRepository
{
    public class ExternalReviewLink
    {
        public ContentReference ContentLink { get; set; }
        public string DisplayName { get; set; }
        public bool IsEditable { get; set; }
        public int? ProjectId { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public string LinkUrl { get; set; }
        public string PinCode { get; set; }
        public string ProjectName { get; set; }
        public string[] VisitorGroups { get; set; }
    }
}
