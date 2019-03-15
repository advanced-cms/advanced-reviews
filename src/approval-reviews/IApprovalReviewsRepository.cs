using EPiServer.Core;

namespace AdvancedApprovalReviews
{
    public interface IApprovalReviewsRepository
    {
        /// <summary>
        /// Save content reviews
        /// </summary>
        /// <param name="contentLink">Version specific content reference</param>
        /// <param name="serializedData">Serialized reviews</param>
        void Save(ContentReference contentLink, string serializedData);


        /// <summary>
        /// Load content reviews
        /// </summary>
        /// <param name="contentLink">Version specific content reference</param>
        /// <returns>Serialized reviews</returns>
        string Load(ContentReference contentLink);
    }
}
