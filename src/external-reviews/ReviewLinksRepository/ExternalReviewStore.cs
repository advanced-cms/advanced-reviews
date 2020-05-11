using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvancedExternalReviews.PinCodeSecurity;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Core;
using EPiServer.Notification;
using EPiServer.Notification.Internal;
using EPiServer.Shell.Services.Rest;
using EPiServer.Web;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    /// <summary>
    /// Manage external review links
    /// </summary>
    [RestStore("externalreviewstore")]
    public class ExternalReviewStore : RestControllerBase
    {
        private readonly IContentLoader _contentLoader;
        private readonly NotificationOptions _notificationOptions;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly EmailNotificationProvider _emailNotificationProvider;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly CurrentProject _currentProject;

        public ExternalReviewStore(IContentLoader contentLoader, NotificationOptions notificationOptions,
            IExternalReviewLinksRepository externalReviewLinksRepository,
            EmailNotificationProvider emailNotificationProvider,
            ExternalReviewOptions externalReviewOptions, CurrentProject currentProject)
        {
            _contentLoader = contentLoader;
            _notificationOptions = notificationOptions;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _emailNotificationProvider = emailNotificationProvider;
            _externalReviewOptions = externalReviewOptions;
            _currentProject = currentProject;
        }


        private void HidePinCode(ExternalReviewLink externalReviewLink)
        {
            if (!string.IsNullOrWhiteSpace(externalReviewLink.PinCode))
            {
                externalReviewLink.PinCode = "****";
            }
        }

        [HttpGet]
        public ActionResult Get(ContentReference id, int? projectId)
        {
            var externalReviewLinks = _externalReviewLinksRepository.GetLinksForContent(id, projectId).ToList();
            foreach (var externalReviewLink in externalReviewLinks)
            {
                HidePinCode(externalReviewLink);
            }
            return Rest(externalReviewLinks);
        }

        [HttpPost]
        public ActionResult Post(PostExternalReviewLinkModel externalLink)
        {
            if (externalLink == null || externalLink.ContentLink == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // should not be possible to add editable link when option is not available
            if (externalLink.IsEditable && !_externalReviewOptions.EditableLinksEnabled)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var validTo = externalLink.IsEditable ? _externalReviewOptions.EditLinkValidTo: _externalReviewOptions.ViewLinkValidTo;
            var result = _externalReviewLinksRepository.AddLink(externalLink.ContentLink, externalLink.IsEditable, validTo, _currentProject.ProjectId);
            HidePinCode(result);
            return Rest(result);
        }

        public ActionResult Edit(string id, DateTime? validTo, string pinCode, string displayName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!string.IsNullOrWhiteSpace(pinCode))
            {
                pinCode = PinCodeHashGenerator.Hash(pinCode, id);
            }

            var result = _externalReviewLinksRepository.UpdateLink(id, validTo, pinCode, displayName);
            HidePinCode(result);

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

            if (!_contentLoader.TryGet(externalReviewLink.ContentLink, out IContent content))
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
