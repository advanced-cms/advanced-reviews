using EPiServer.ServiceLocation;

namespace AdvancedApprovalReviews
{
    [Options]
    public class ApprovalOptions
    {
        public NotificationsOptions Notifications { get; private set; } = new NotificationsOptions();
    }

    public class NotificationsOptions
    {
        public bool NotificationsEnabled { get; set; } = true;
    }
}
