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
        IEnumerable<ExternalReviewLink> GetLinksForContent(ContentReference contentLink);
        ExternalReviewLink GetContentByToken(string token);
        ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo);
        void DeleteLink(string token);
    }

    [ServiceConfiguration(typeof(IExternalReviewLinksRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ExternalReviewLinksRepository: IExternalReviewLinksRepository
    {
        private readonly ExternalReviewLinkBuilder _externalReviewLinkBuilder;
        private readonly DynamicDataStoreFactory _dataStoreFactory;
        private readonly ExternalReviewOptions _externalReviewOptions;

        public ExternalReviewLinksRepository(ExternalReviewLinkBuilder externalReviewLinkBuilder, DynamicDataStoreFactory dataStoreFactory, ExternalReviewOptions externalReviewOptions)
        {
            _externalReviewLinkBuilder = externalReviewLinkBuilder;
            _dataStoreFactory = dataStoreFactory;
            _externalReviewOptions = externalReviewOptions;
        }

        public IEnumerable<ExternalReviewLink> GetLinksForContent(ContentReference contentLink)
        {
            return GetStore().Items<ExternalReviewLinkDds>().Where(x => x.ContentLink == contentLink).ToList().Select(
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

        public ExternalReviewLink AddLink(ContentReference contentLink, bool isEditable, TimeSpan validTo)
        {
            var externalReviewLink = new ExternalReviewLinkDds
            {
                ContentLink = contentLink,
                IsEditable = isEditable,
                Token = Guid.NewGuid().ToString(),
                ValidTo = DateTime.Now.Add(validTo)
            };
            GetStore().Save(externalReviewLink);
            return _externalReviewLinkBuilder.FromExternalReview(externalReviewLink);
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
