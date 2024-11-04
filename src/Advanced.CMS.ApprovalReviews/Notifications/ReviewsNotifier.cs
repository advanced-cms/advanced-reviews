using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Notification;
using EPiServer.Security;

namespace Advanced.CMS.ApprovalReviews.Notifications;

public class ReviewsNotifier
{
    public const string ChannelName = "external-review";

    private readonly ApprovalOptions _options;
    private readonly IContentLoader _contentLoader;
    private readonly IPrincipalAccessor _principalAccessor;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IObjectSerializer _objectSerializer;
    private readonly INotifier _notifier;
    private readonly ReviewLocationParser _reviewLocationParser;

    public ReviewsNotifier(ApprovalOptions options,
        IContentLoader contentLoader,
        IPrincipalAccessor principalAccessor,
        ISubscriptionService subscriptionService,
        IObjectSerializer objectSerializer,
        INotifier notifier, ReviewLocationParser reviewLocationParser)
    {
        _options = options;
        _contentLoader = contentLoader;
        _principalAccessor = principalAccessor;
        _subscriptionService = subscriptionService;
        _objectSerializer = objectSerializer;
        _notifier = notifier;
        _reviewLocationParser = reviewLocationParser;
    }

    public async Task<bool> NotifyCmsEditor(ContentReference contentLink, string token, string data, bool isSenderEditModeUser)
    {
        if (!_options.Notifications.NotificationsEnabled)
        {
            return false;
        }

        var contentVersion = _contentLoader.Get<IContent>(contentLink);
        if (contentVersion == null)
        {
            return false;
        }

        var comment = _reviewLocationParser.GetLastComment(data);

        var notificationReceiver = (contentVersion as IChangeTrackable).ChangedBy;
        var users = new List<string>
        {
            notificationReceiver
        };
        if (isSenderEditModeUser)
        {
            users.Add(comment.Author);
        }

        var subscribers = users.Select(x => new NotificationUser(x)).ToList();

        // subscribe users to comment
        var subscriptionKey = new Uri($"advancedreviews://notification/{token}");
        var userName = _principalAccessor.CurrentName();
        await _subscriptionService.SubscribeAsync(subscriptionKey, subscribers).ConfigureAwait(false);

        var recipients = (await _subscriptionService.ListSubscribersAsync(subscriptionKey).ConfigureAwait(false))
            .Where(u => !u.UserName.Equals(comment.Author, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!recipients.Any())
        {
            return false;
        }

        // send message
        var model = new ReviewContentNotificationModel
        {
            Title = contentVersion.Name,
            ContentLink = contentLink,
            SenderDisplayName = comment.Author,
            Text = comment.Text
        };

        var notificationMessage = new NotificationMessage
        {
            ChannelName = ChannelName,
            Sender = new NotificationUser(userName),
            //Sender = new NotificationUser(_principalAccessor.CurrentName()),
            Recipients = recipients,
            Content = _objectSerializer.Serialize(model)
        };

        try
        {
            await _notifier.PostNotificationAsync(notificationMessage).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }

        return true;
    }
}

internal class ReviewLocationDto
{
    public IEnumerable<CommentDto> Comments { get; set; }

    public CommentDto FirstComment { get; set; }
}

public class CommentDto
{
    public string Author { get; set; }

    public string Text { get; set; }
}

public class ReviewLocationParser
{
    private readonly IObjectSerializer _objectSerializer;

    public ReviewLocationParser(IObjectSerializerFactory objectSerializerFactory)
    {
        _objectSerializer = objectSerializerFactory.GetSerializer(KnownContentTypes.Json);
    }

    public CommentDto GetLastComment(string data)
    {
        var dto = _objectSerializer.Deserialize<ReviewLocationDto>(data);
        if (dto.Comments == null || dto.Comments.Any() == false)
        {
            return dto.FirstComment;
        }

        return dto.Comments.FirstOrDefault();
    }
}
