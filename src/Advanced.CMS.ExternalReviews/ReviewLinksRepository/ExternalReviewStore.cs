using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Advanced.CMS.ApprovalReviews;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Core;
using EPiServer.Notification;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ExternalReviews.ReviewLinksRepository
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
        private readonly INotificationProvider _emailNotificationProvider;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly CurrentProject _currentProject;
        private readonly ISiteUriResolver _siteUriResolver;

        public ExternalReviewStore(IContentLoader contentLoader, NotificationOptions notificationOptions,
            IExternalReviewLinksRepository externalReviewLinksRepository,
            INotificationProvider emailNotificationProvider,
            ExternalReviewOptions externalReviewOptions, CurrentProject currentProject,
            ISiteUriResolver siteUriResolver)
        {
            _contentLoader = contentLoader;
            _notificationOptions = notificationOptions;
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _emailNotificationProvider = emailNotificationProvider;
            _externalReviewOptions = externalReviewOptions;
            _currentProject = currentProject;
            _siteUriResolver = siteUriResolver;
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
        public ActionResult Post([FromBody] PostExternalReviewLinkModel externalLink)
        {
            if (externalLink?.ContentLink == null || !ContentReference.TryParse(externalLink.ContentLink, out var contentLink))
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // should not be possible to add editable link when option is not available
            if (externalLink.IsEditable && !_externalReviewOptions.EditableLinksEnabled)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var validTo = externalLink.IsEditable ? _externalReviewOptions.EditLinkValidTo: _externalReviewOptions.ViewLinkValidTo;
            var result = _externalReviewLinksRepository.AddLink(contentLink, externalLink.IsEditable, validTo, _currentProject.ProjectId);
            HidePinCode(result);
            return Rest(result);
        }

        [HttpPost]
        public ActionResult Edit(string id, [FromBody] PinDto pinDto)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var result = _externalReviewLinksRepository.UpdateLink(id, pinDto.ValidTo, pinDto.PinCode, pinDto.DisplayName, pinDto.VisitorGroups);
            HidePinCode(result);

            return Rest(result);
        }

        [HttpPost]
        public async Task<ActionResult> ShareReviewLink(string id, [FromBody] ShareEmailDto dto)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            if (string.IsNullOrWhiteSpace(_notificationOptions.NotificationEmailAddress))
            {
                return new BadRequestObjectResult("Sender email address is not configured. Contact with system administrator");
            }

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(id);
            if (externalReviewLink == null)
            {
                return new NotFoundObjectResult("Token not found");
            }

            if (!_contentLoader.TryGet(externalReviewLink.ContentLink, out IContent _))
            {
                return new NotFoundObjectResult("Content not found");
            }

            await SendMail(externalReviewLink, dto.Email, dto.Subject, dto.Message);

            return Rest(true);
        }

        private async Task<bool> SendMail(ExternalReviewLink externalReviewLink, string email, string subject,
            string message)
        {
            var linkUrl = new Uri(_siteUriResolver.GetUri(externalReviewLink.ContentLink), externalReviewLink.LinkUrl);

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
#pragma warning disable 618
            await _emailNotificationProvider.SendAsync(providerNotificationMessages, msg => { result = true; },
#pragma warning restore 618
                (_, _) => { result = false; }).ConfigureAwait(true);
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
        public string ContentLink { get; set; }
        public bool IsEditable { get; set; }
    }

    public class ShareEmailDto
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class PinDto
    {
        public DateTime? ValidTo { get; set; }
        public string PinCode { get; set; }
        public string DisplayName { get; set; }
        public string[] VisitorGroups { get; set; }
    }
}
