using System.Collections.Generic;
using EPiServer.Core;

namespace AdvancedApprovalReviews
{
    public interface IApprovalReviewsRepository
    {
        /// <summary>
        /// Save content reviews
        /// </summary>
        /// <param name="contentLink">Version specific content reference</param>
        /// <param name="reviewLocations">List of review locations</param>
        void Save(ContentReference contentLink, IEnumerable<ReviewLocation> reviewLocations);

        /// <summary>
        /// Updates list of review locations for ContentLink
        /// </summary>
        /// <param name="contentLink">Version specific content reference</param>
        /// <param name="reviewLocation">Review Location</param>
        ReviewLocation Update(ContentReference contentLink, ReviewLocation reviewLocation);
        
        /// <summary>
        /// Load content reviews
        /// </summary>
        /// <param name="contentLink">Version specific content reference</param>
        /// <returns>Serialized reviews</returns>
        IEnumerable<ReviewLocation> Load(ContentReference contentLink);
    }

    public class ReviewLocation
    {
        public string Id { get; set; }
        public string Data { get; set; }
    }
}
