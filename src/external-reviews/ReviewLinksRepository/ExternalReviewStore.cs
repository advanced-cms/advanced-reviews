using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Notification;
using EPiServer.Notification.Internal;
using EPiServer.Shell.Services.Rest;
using EPiServer.Web;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    [RestStore("externalreviewstore")]
    public class ExternalReviewStore : RestControllerBase
    {
        private readonly IContentLoader _contentLoader;
        private readonly NotificationOptions _notificationOptions;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly EmailNotificationProvider _emailNotificationProvider;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public ExternalReviewStore(IContentLoader contentLoader, NotificationOptions notificationOptions,
            IExternalReviewLinksRepository externalReviewLinksRepository,
            EmailNotificationProvider emailNotificationProvider,
            ExternalReviewOptions externalReviewOptions)
        {
            _contentLoader = contentLoader;
            _notificationOptions = notificationOptions;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _emailNotificationProvider = emailNotificationProvider;
            _externalReviewOptions = externalReviewOptions;
        }

        [HttpGet]
        public ActionResult Get(ContentReference id)
        {
            return Rest(_externalReviewLinksRepository.GetLinksForContent(id));
        }

        [HttpPost]
        public ActionResult Post(PostExternalReviewLinkModel externalLink)
        {
            if (externalLink == null || externalLink.ContentLink == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (externalLink.IsEditable && !_externalReviewOptions.EditableLinksEnabled)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = _externalReviewLinksRepository.AddLink(externalLink.ContentLink, externalLink.IsEditable);
            return Rest(result);
        }

        public async Task<ActionResult> ShareReviewLink(string id, string email, string subject, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(_notificationOptions.NotificationEmailAddress))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sender email address is not configured. Contact with system administrator");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(id);
            if (externalReviewLink == null)
            {
                return new HttpNotFoundResult("Token not found");
            }
            
            if (!_contentLoader.TryGet<IContent>(externalReviewLink.ContentLink, out IContent content))
            {
                return new HttpNotFoundResult("Content not found");
            }

            await SendMail(externalReviewLink, email, subject, message);

            return Rest(true);
        }

        private async Task<bool> SendMail(ExternalReviewLink externalReviewLink, string email, string subject,
            string message)
        {
            var linkUrl = new Uri(SiteDefinition.Current.SiteUrl, externalReviewLink.LinkUrl);

            message = message.Replace("[#link#]", linkUrl.ToString());

            var providerNotificationMessages = new List<ProviderNotificationMessage>
            {
                new ProviderNotificationMessage
                {
                    Content = message,
                    RecipientAddresses = new[] {email},
                    SenderAddress = _notificationOptions.NotificationEmailAddress,
                    Subject = subject,
                    SenderDisplayName = _notificationOptions.NotificationEmailDisplayName
                }
            };
            var result = true;
            await _emailNotificationProvider.SendAsync(providerNotificationMessages, msg => { result = true; },
                (msg, exception) => { result = false; }).ConfigureAwait(true);
            return result;
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            _externalReviewLinksRepository.DeleteLink(id);
            return Rest(true);
        }
    }

    public class PostExternalReviewLinkModel
    {
        public ContentReference ContentLink { get; set; }
        public bool IsEditable { get; set; }
    }
}
