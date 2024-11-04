using EPiServer.ServiceLocation;

namespace Advanced.CMS.ApprovalReviews;

[Options]
public class ApprovalOptions
{
    public NotificationsOptions Notifications { get; private set; } = new();
}

public class NotificationsOptions
{
    public bool NotificationsEnabled { get; set; } = true;
}
