﻿using System;
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
        /// Remove a specific review location
        /// </summary>
        /// <param name="id">Id of the review location</param>
        /// <param name="contentLink">Version specific content reference</param>
        void RemoveReviewLocation(string id, ContentReference contentLink);

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

        IEnumerable<ApprovalReview> LoadAll();

        void Delete(ContentReference contentLink);
    }

    public class ReviewLocation
    {
        public string Id { get; set; }
        public string Data { get; set; }
    }
}
