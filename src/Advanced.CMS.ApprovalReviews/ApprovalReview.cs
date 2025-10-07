using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Advanced.CMS.ApprovalReviews;

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
public class ApprovalReview : IDynamicData
{
    public Identity Id { get; set; }

    [EPiServerDataIndex]
    public ContentReference ContentLink { get; set; }

    public string SerializedReview { get; set; }
}
