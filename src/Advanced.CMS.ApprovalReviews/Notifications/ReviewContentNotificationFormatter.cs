using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Cms.Shell.UI.Notifications;
using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Notification;
using EPiServer.Web.Routing;

namespace Advanced.CMS.ApprovalReviews.Notifications;

internal class ReviewContentNotificationFormatter : IUserNotificationFormatter
{
    private readonly IObjectSerializer _objectSerializer;
    private readonly EditUrlResolver _editUrlResolver;

    public ReviewContentNotificationFormatter(IObjectSerializerFactory objectSerializerFactory, EditUrlResolver editUrlResolver)
    {
        _objectSerializer = objectSerializerFactory.GetSerializer(KnownContentTypes.Json);
        _editUrlResolver = editUrlResolver;
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
            result.Link = GetEditUri(reviewContent.ContentLink);
        }

        var userNameContainer = "<span class='epi-username external-review'>{0}</span>";
        var userName = reviewContent.SenderDisplayName ?? "external editor";
        userName = string.Format(userNameContainer, userName);

        result.Content = $"{userName} added new comment: \"'{reviewContent.Text?.Ellipsis(50)}'\"";

        return Task.FromResult(result);
    }

    internal Uri GetEditUri(ContentReference contentLink, string contextKey = "epi.cms.contentdata")
    {
        var editUrlBuilder = new UrlBuilder(_editUrlResolver.GetEditViewUrl(contentLink, new EditUrlArguments()));
        editUrlBuilder.Fragment = $"context={contextKey}:///{contentLink}";
        return editUrlBuilder.Uri;
    }
}
