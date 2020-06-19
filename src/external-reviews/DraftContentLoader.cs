using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvancedExternalReviews.DraftContentAreaPreview;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    public class DraftContentLoader : IContentLoader
    {
        private readonly IContentLoader _defaultContentLoader;
        private readonly ServiceAccessor<ReviewsContentLoader> _reviewsContentLoader;

        public DraftContentLoader(IContentLoader defaultContentLoader, ServiceAccessor<ReviewsContentLoader> reviewsContentLoader)
        {
            _defaultContentLoader = defaultContentLoader;
            _reviewsContentLoader = reviewsContentLoader;
        }

        public T Get<T>(Guid contentGuid) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentGuid);
        }

        public T Get<T>(Guid contentGuid, LoaderOptions settings) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentGuid, settings);
        }

        public T Get<T>(Guid contentGuid, CultureInfo language) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentGuid, language);
        }

        public T Get<T>(ContentReference contentLink) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentLink);
        }

        public T Get<T>(ContentReference contentLink, CultureInfo language) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentLink, language);
        }

        public T Get<T>(ContentReference contentLink, LoaderOptions settings) where T : IContentData
        {
            return _defaultContentLoader.Get<T>(contentLink, settings);
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink) where T : IContentData
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink).ToList();
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language) where T : IContentData
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, language);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, language);
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings) where T : IContentData
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, settings);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, settings);
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language, int startIndex, int maxRows) where T : IContentData
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, language, startIndex, maxRows);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, language, startIndex, maxRows);
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings, int startIndex, int maxRows) where T : IContentData
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, settings, startIndex, maxRows);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, settings, startIndex, maxRows);
        }

        public IEnumerable<ContentReference> GetDescendents(ContentReference contentLink)
        {
            return _defaultContentLoader.GetDescendents(contentLink);
        }

        public IEnumerable<IContent> GetAncestors(ContentReference contentLink)
        {
            return _defaultContentLoader.GetAncestors(contentLink);
        }

        public IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, CultureInfo language)
        {
            return _defaultContentLoader.GetItems(contentLinks, language);
        }

        public IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, LoaderOptions settings)
        {
            return _defaultContentLoader.GetItems(contentLinks, settings);
        }

        public IContent GetBySegment(ContentReference parentLink, string urlSegment, CultureInfo language)
        {
            return _defaultContentLoader.GetBySegment(parentLink, urlSegment, language);
        }

        public IContent GetBySegment(ContentReference parentLink, string urlSegment, LoaderOptions settings)
        {
            return _defaultContentLoader.GetBySegment(parentLink, urlSegment, settings);
        }

        public bool TryGet<T>(ContentReference contentLink, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentLink, out content);
        }

        public bool TryGet<T>(ContentReference contentLink, CultureInfo language, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentLink, language, out content);
        }

        public bool TryGet<T>(ContentReference contentLink, LoaderOptions settings, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentLink, settings, out content);
        }

        public bool TryGet<T>(Guid contentGuid, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentGuid, out content);
        }

        public bool TryGet<T>(Guid contentGuid, CultureInfo language, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentGuid, language, out content);
        }

        public bool TryGet<T>(Guid contentGuid, LoaderOptions loaderOptions, out T content) where T : IContentData
        {
            return _defaultContentLoader.TryGet(contentGuid, loaderOptions, out content);
        }
    }
}
