using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Notification;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews.Notifications
{
    [ServiceConfiguration(typeof(ReviewsNotifier))]
    public class ReviewsNotifier
    {
        public const string ChannelName = "external-review";

        private readonly ExternalReviewOptions _options;
        private readonly IContentVersionRepository _contentVersionRepository;
        private readonly IPrincipalAccessor _principalAccessor;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IObjectSerializer _objectSerializer;
        private readonly INotifier _notifier;

        public ReviewsNotifier(ExternalReviewOptions options,
            IContentVersionRepository contentVersionRepository,
            IPrincipalAccessor principalAccessor,
            ISubscriptionService subscriptionService,
            IObjectSerializer objectSerializer,
            INotifier notifier)
        {
            _options = options;
            _contentVersionRepository = contentVersionRepository;
            _principalAccessor = principalAccessor;
            _subscriptionService = subscriptionService;
            _objectSerializer = objectSerializer;
            _notifier = notifier;
        }

        public async Task<bool> NotifyCmsEditor(ContentReference contentLink, string token, string senderUser)
        {
            if (!_options.Notifications.NotificationsEnabled)
            {
                return false;
            }

            var contentVersion = _contentVersionRepository.Load(contentLink);
            if (contentVersion == null)
            {
                return false;
            }

            var notificationReceiver = contentVersion.SavedBy;
            //var notificationSender = _principalAccessor.CurrentName();
            var users = new List<string>
            {
                notificationReceiver
            };

            var subscribers = users.Select(x => new NotificationUser(x)).ToList();

            // subscribe users to comment
            var subscriptionKey = new Uri($"projects://notification/{token}");

            await _subscriptionService.SubscribeAsync(subscriptionKey, subscribers).ConfigureAwait(false);

            var recipients = (await _subscriptionService.ListSubscribersAsync(subscriptionKey).ConfigureAwait(false))
                //.Where(u => !u.UserName.Equals(senderUser.Identity.Name, StringComparison.OrdinalIgnoreCase))
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
                SenderDisplayName = senderUser
            };

            var notificationMessage = new NotificationMessage()
            {
                ChannelName = ChannelName,
                Sender = new NotificationUser(_principalAccessor.CurrentName()),
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
}
