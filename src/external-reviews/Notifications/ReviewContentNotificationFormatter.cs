using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Notification;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews.Notifications
{
    [ServiceConfiguration(typeof(IUserNotificationFormatter))]
    internal class ReviewContentNotificationFormatter : IUserNotificationFormatter
    {
        private readonly IObjectSerializer _objectSerializer;

        public ReviewContentNotificationFormatter(IObjectSerializerFactory objectSerializerFactory)
        {
            _objectSerializer = objectSerializerFactory.GetSerializer(KnownContentTypes.Json);
        }

        public static string FeatureIconKey => "featureIcon";

        /// <summary>
        /// Gets the list of channels supported by this formatter
        /// </summary>
        public IEnumerable<string> SupportedChannelNames => new[] { ReviewsNotifier.ChannelName };

        protected UserNotificationMessage CreateBasicFormattedUserNotificationMessage(UserNotificationMessage message)
        {
            return new UserNotificationMessage
            {
                Id = message.Id,
                Posted = message.Posted,
                Sender = message.Sender,
                Recipient = message.Recipient,
                Read = message.Read
            };
        }

        public Task<UserNotificationMessage> FormatUserMessageAsync(UserNotificationMessage message)
        {
            var result = CreateBasicFormattedUserNotificationMessage(message);

            ReviewContentNotificationModel reviewContent;
            try
            {
                reviewContent = _objectSerializer.Deserialize<ReviewContentNotificationModel>(message.Content);
            }
            catch (Exception)
            {
                reviewContent = new ReviewContentNotificationModel();
            }

            if (reviewContent == null)
            {
                return Task.FromResult(result);
            }

            result.Subject = reviewContent.Title;
            if (!ContentReference.IsNullOrEmpty(reviewContent.ContentLink))
            {
                result.Link = new Uri("epi.cms.contentdata:///" + reviewContent.ContentLink);
            }

            var userNameContainer = "<span class='epi-username'>{0}</span>";
            var userName = reviewContent.SenderDisplayName ?? "external editor";
            userName = string.Format(userNameContainer, userName);

            result.Content = $"{userName} added new comment for '{reviewContent.Title}'";

            return Task.FromResult(result);
        }
    }
}
