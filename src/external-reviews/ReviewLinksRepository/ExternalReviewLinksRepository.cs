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
        int RemoveExpiredLinks();
        bool HasPinCode(string token);
        ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo, int? projectId);

        /// <summary>
        /// Udate link
        /// </summary>
        /// <param name="token"></param>
        /// <param name="validTo"></param>
        /// <param name="pinCode">New PIN code. If null then PIN is not updated</param>
        /// <param name="displayName">Link display name, when empty then fallback to token</param>
        /// <param name="visitorGroups">Impersonate with the following visitor groups ids</param>
        ExternalReviewLink UpdateLink(string token, DateTime? validTo, string pinCode, string displayName, string[] visitorGroups);

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

        public int RemoveExpiredLinks()
        {
            var store = GetStore();
            var expiredItems = store.Items<ExternalReviewLinkDds>().Where(x => x.ValidTo < DateTime.Now).ToList();
            foreach (var item in expiredItems)
            {
                store.Delete(item.Id);
            }

            return expiredItems.Count;
        }

        private ExternalReviewLinkDds GetReviewLinkDds(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            token = token.ToLower();
            return GetStore().Items<ExternalReviewLinkDds>().FirstOrDefault(x => x.Token == token);
        }

        public bool HasPinCode(string token)
        {
            var externalReviewLinkDds = GetReviewLinkDds(token);
            if (externalReviewLinkDds == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(externalReviewLinkDds.PinCode);
        }

        public ExternalReviewLink GetContentByToken(string token)
        {
            var externalReviewLinkDds = GetReviewLinkDds(token);
            return externalReviewLinkDds == null
                ? null
                : _externalReviewLinkBuilder.FromExternalReview(externalReviewLinkDds);
        }

        public ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo,
            int? projectId)
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

        public ExternalReviewLink UpdateLink(string token, DateTime? validTo, string pinCode, string displayName, string[] visitorGroups)
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
            item.VisitorGroups = visitorGroups ?? new[] {"cc5fc022-4186-431e-b38a-e257d8cafd51"};

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
