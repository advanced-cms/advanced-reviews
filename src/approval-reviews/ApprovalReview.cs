using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace AdvancedApprovalReviews
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ApprovalReview : IDynamicData
    {
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public ContentReference ContentLink { get; set; }

        public string SerializedReview { get; set; }
    }
}
