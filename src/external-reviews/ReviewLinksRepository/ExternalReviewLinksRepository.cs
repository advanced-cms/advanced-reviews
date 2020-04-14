using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    public interface IExternalReviewLinksRepository
    {
        IEnumerable<ExternalReviewLink> GetLinksForContent(ContentReference contentLink, int? projectId);
        ExternalReviewLink GetContentByToken(string token);
        ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo, int? projectId);

        /// <summary>
        /// Udate link
        /// </summary>
        /// <param name="token"></param>
        /// <param name="validTo"></param>
        /// <param name="pinCode">New PIN code. If null then PIN is not updated</param>
        /// <param name="displayName">Link display name, when empty then fallback to token</param>
        ExternalReviewLink UpdateLink(string token, DateTime? validTo, string pinCode, string displayName);

        void DeleteLink(string token);
    }

    [ServiceConfiguration(typeof(IExternalReviewLinksRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ExternalReviewLinksRepository: IExternalReviewLinksRepository
    {
        private readonly ExternalReviewLinkBuilder _externalReviewLinkBuilder;
        private readonly DynamicDataStoreFactory _dataStoreFactory;

        public ExternalReviewLinksRepository(ExternalReviewLinkBuilder externalReviewLinkBuilder,
            DynamicDataStoreFactory dataStoreFactory)
        {
            _externalReviewLinkBuilder = externalReviewLinkBuilder;
            _dataStoreFactory = dataStoreFactory;
        }

        public IEnumerable<ExternalReviewLink> GetLinksForContent(ContentReference contentLink, int? projectId)
        {
            if (!projectId.HasValue)
            {
                // when project is null, then set it to -1
                // to make sure that it won't return all links with no projects set
                projectId = -1;
            }
            return GetStore().Items<ExternalReviewLinkDds>().Where(x => x.ContentLink == contentLink || x.ProjectId == projectId).ToList().Select(
                _externalReviewLinkBuilder.FromExternalReview);
        }

        public ExternalReviewLink GetContentByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            token = token.ToLower();
            var externalReviewLinkDds = GetStore().Items<ExternalReviewLinkDds>()
                .FirstOrDefault(x => x.Token == token);
            if (externalReviewLinkDds == null)
            {
                return null;
            }
            return _externalReviewLinkBuilder.FromExternalReview(externalReviewLinkDds);
        }

        public ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo, int? projectId)
        {
            var externalReviewLink = new ExternalReviewLinkDds
            {
                ContentLink = contentLink,
                IsEditable = isEditable,
                ProjectId = projectId,
                Token = Guid.NewGuid().ToString(),
                ValidTo = DateTime.Now.Add(validTo)
            };
            GetStore().Save(externalReviewLink);
            return _externalReviewLinkBuilder.FromExternalReview(externalReviewLink);
        }

        public ExternalReviewLink UpdateLink(string token, DateTime? validTo, string pinCode, string displayName)
        {
            var store = GetStore();
            var item = store.Items<ExternalReviewLinkDds>()
                .FirstOrDefault(x => x.Token == token);
            if (item == null)
            {
                return null;
            }

            if (validTo.HasValue)
            {
                item.ValidTo = validTo.Value;
            }

            if (pinCode != null)
            {
                item.PinCode = pinCode;
            }

            item.DisplayName = displayName;

            store.Save(item);

            return _externalReviewLinkBuilder.FromExternalReview(item);
        }

        public void DeleteLink(string token)
        {
            var item = GetStore().Items<ExternalReviewLinkDds>()
                .FirstOrDefault(x => x.Token == token);
            if (item == null)
            {
                return;
            }
            GetStore().Delete(item.Id);
        }

        private DynamicDataStore GetStore()
        {
            return _dataStoreFactory.GetStore(typeof(ExternalReviewLinkDds)) ?? _dataStoreFactory.CreateStore(typeof(ExternalReviewLinkDds));
        }

    }
}
