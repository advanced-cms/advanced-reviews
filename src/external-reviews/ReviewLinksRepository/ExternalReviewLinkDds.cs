using System;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ExternalReviewLinkDds : IDynamicData
    {
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public ContentReference ContentLink { get; set; }

        public bool IsEditable { get; set; }

        [EPiServerDataIndex]
        public string Token { get; set; }

        public DateTime ValidTo { get; set; }
    }
}
