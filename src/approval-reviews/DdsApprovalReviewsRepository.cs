using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace AdvancedApprovalReviews
{
    [ServiceConfiguration(typeof(IApprovalReviewsRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DdsApprovalReviewsRepository: IApprovalReviewsRepository
    {
        private readonly object _lock = new object();

        private readonly DynamicDataStoreFactory _dataStoreFactory;

        public DdsApprovalReviewsRepository(DynamicDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public void Save(ContentReference contentLink, string serializedReview)
        {
            lock (_lock)
            {
                var approvalReview = LoadApprovalReview(contentLink) ?? new ApprovalReview
                {
                    ContentLink = contentLink
                };

                approvalReview.SerializedReview = serializedReview;
                GetStore().Save(approvalReview);
            }
        }

        public string Load(ContentReference contentLink)
        {
            var approvalReview = LoadApprovalReview(contentLink);
            return approvalReview?.SerializedReview;
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
}
