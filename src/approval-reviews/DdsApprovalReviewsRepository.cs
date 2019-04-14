using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.Framework.Serialization;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;

namespace AdvancedApprovalReviews
{
    [ServiceConfiguration(typeof(IApprovalReviewsRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DdsApprovalReviewsRepository: IApprovalReviewsRepository
    {
        private readonly object _lock = new object();
        private static readonly ILog _log = LogManager.GetLogger(typeof(DdsApprovalReviewsRepository));

        private readonly DynamicDataStoreFactory _dataStoreFactory;
        private readonly IObjectSerializerFactory _serializerFactory;

        public DdsApprovalReviewsRepository(DynamicDataStoreFactory dataStoreFactory, IObjectSerializerFactory serializerFactory)
        {
            _dataStoreFactory = dataStoreFactory;
            _serializerFactory = serializerFactory;
        }

        public void Save(ContentReference contentLink, IEnumerable<ReviewLocation> reviewLocations)
        {
            var objectSerializer = _serializerFactory.GetSerializer(KnownContentTypes.Json);
            lock (_lock)
            {
                var approvalReview = LoadApprovalReview(contentLink) ?? new ApprovalReview
                {
                    ContentLink = contentLink
                };
                approvalReview.SerializedReview = objectSerializer.Serialize(reviewLocations);
                GetStore().Save(approvalReview);
            }
        }

        public IEnumerable<ReviewLocation> Load(ContentReference contentLink)
        {
            var approvalReview = LoadApprovalReview(contentLink);
            var data = approvalReview?.SerializedReview;
            if (string.IsNullOrWhiteSpace(data))
            {
                return Enumerable.Empty<ReviewLocation>();
            }

            try
            {
                var reviewLocations = _serializerFactory.GetSerializer(KnownContentTypes.Json).Deserialize<IEnumerable<ReviewLocation>>(data);
                return reviewLocations;
            }
            catch (Exception ex)
            {
                _log.Debug("Access denied while deleting", ex);
                return Enumerable.Empty<ReviewLocation>();
            }
        }

        public IEnumerable<ApprovalReview> LoadAll()
        {
            return GetStore().Items<ApprovalReview>();
        }

        public void Delete(ContentReference contentLink)
        {
            var review = GetStore().Items<ApprovalReview>().FirstOrDefault(x => x.ContentLink == contentLink);
            if (review == null)
            {
                throw new ArgumentOutOfRangeException($"Can't find review location for: {contentLink}");
            }
            GetStore().Delete(review.Id);
        }

        public ReviewLocation Update(ContentReference contentLink, ReviewLocation reviewLocation)
        {
            var data = reviewLocation.Data;

            lock (_lock)
            {
                var reviewLocations = Load(contentLink).ToList();

                if (string.IsNullOrWhiteSpace(reviewLocation.Id))
                {
                    reviewLocation = new ReviewLocation
                    {
                        Id = Guid.NewGuid().ToString(),
                    };
                    reviewLocations.Add(reviewLocation);
                }
                else
                {
                    reviewLocation = reviewLocations.FirstOrDefault(x => x.Id == reviewLocation.Id);
                    if (reviewLocation == null)
                    {
                        throw new ReviewLocationNotFoundException();
                    }
                }

                reviewLocation.Data = data;
                Save(contentLink, reviewLocations);
            }

            return reviewLocation;
        }

        private ApprovalReview LoadApprovalReview(ContentReference contentLink)
        {
            return GetStore().Items<ApprovalReview>().FirstOrDefault(x => x.ContentLink == contentLink);
        }

        private DynamicDataStore GetStore()
        {
            return _dataStoreFactory.GetStore(typeof(ApprovalReview)) ?? _dataStoreFactory.CreateStore(typeof(ApprovalReview));
        }
    }

    public class ReviewLocationNotFoundException : Exception
    {
    }
}
