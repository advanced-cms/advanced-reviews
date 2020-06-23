using EPiServer.Core;

namespace AdvancedApprovalReviews.Notifications
{
    public class ReviewContentNotificationModel
    {
        public ContentReference ContentLink { get; set; }

        public string SenderDisplayName { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
    }
}
