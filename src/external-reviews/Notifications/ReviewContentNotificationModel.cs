using EPiServer.Core;

namespace AdvancedExternalReviews.Notifications
{
    public class ReviewContentNotificationModel
    {
        public ContentReference ContentLink { get; set; }

        public string SenderDisplayName { get; set; }

        public string Title { get; set; }
    }
}
