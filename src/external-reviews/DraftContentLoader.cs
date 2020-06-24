using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvancedExternalReviews.DraftContentAreaPreview;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    public class DraftContentLoader : ContentLoader
    {
        private readonly IContentLoader _defaultContentLoader;
        private readonly ServiceAccessor<ReviewsContentLoader> _reviewsContentLoader;

        public DraftContentLoader(IContentLoader defaultContentLoader, ServiceAccessor<ReviewsContentLoader> reviewsContentLoader)
        {
            _defaultContentLoader = defaultContentLoader;
            _reviewsContentLoader = reviewsContentLoader;
        }

        public override T Get<T>(Guid contentGuid)
        {
            return _defaultContentLoader.Get<T>(contentGuid);
        }

        public override T Get<T>(Guid contentGuid, LoaderOptions settings)
        {
            return _defaultContentLoader.Get<T>(contentGuid, settings);
        }

        public override T Get<T>(Guid contentGuid, CultureInfo language)
        {
            return _defaultContentLoader.Get<T>(contentGuid, language);
        }

        public override T Get<T>(ContentReference contentLink)
        {
            return _defaultContentLoader.Get<T>(contentLink);
        }

        public override T Get<T>(ContentReference contentLink, CultureInfo language)
        {
            return _defaultContentLoader.Get<T>(contentLink, language);
        }

        public override T Get<T>(ContentReference contentLink, LoaderOptions settings)
        {
            return _defaultContentLoader.Get<T>(contentLink, settings);
        }

        public override IEnumerable<T> GetChildren<T>(ContentReference contentLink)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink).ToList();
        }

        public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, language);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, language);
        }

        public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, settings);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, settings);
        }

        public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, CultureInfo language, int startIndex, int maxRows)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, language, startIndex, maxRows);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, language, startIndex, maxRows);
        }

        public override IEnumerable<T> GetChildren<T>(ContentReference contentLink, LoaderOptions settings, int startIndex, int maxRows)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return _defaultContentLoader.GetChildren<T>(contentLink, settings, startIndex, maxRows);
            }

            return _reviewsContentLoader().GetChildrenWithReviews<T>(contentLink, settings, startIndex, maxRows);
        }

        public override IEnumerable<ContentReference> GetDescendents(ContentReference contentLink)
        {
            return _defaultContentLoader.GetDescendents(contentLink);
        }

        public override IEnumerable<IContent> GetAncestors(ContentReference contentLink)
        {
            return _defaultContentLoader.GetAncestors(contentLink);
        }

        public override IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, CultureInfo language)
        {
            return _defaultContentLoader.GetItems(contentLinks, language);
        }

        public override IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, LoaderOptions settings)
        {
            return _defaultContentLoader.GetItems(contentLinks, settings);
        }

        public override IContent GetBySegment(ContentReference parentLink, string urlSegment, CultureInfo language)
        {
            return _defaultContentLoader.GetBySegment(parentLink, urlSegment, language);
        }

        public override IContent GetBySegment(ContentReference parentLink, string urlSegment, LoaderOptions settings)
        {
            return _defaultContentLoader.GetBySegment(parentLink, urlSegment, settings);
        }

        public override bool TryGet<T>(ContentReference contentLink, out T content)
        {
            return _defaultContentLoader.TryGet(contentLink, out content);
        }

        public override bool TryGet<T>(ContentReference contentLink, CultureInfo language, out T content)
        {
            return _defaultContentLoader.TryGet(contentLink, language, out content);
        }

        public override bool TryGet<T>(ContentReference contentLink, LoaderOptions settings, out T content)
        {
            return _defaultContentLoader.TryGet(contentLink, settings, out content);
        }

        public override bool TryGet<T>(Guid contentGuid, out T content)
        {
            return _defaultContentLoader.TryGet(contentGuid, out content);
        }

        public override bool TryGet<T>(Guid contentGuid, CultureInfo language, out T content)
        {
            return _defaultContentLoader.TryGet(contentGuid, language, out content);
        }

        public override bool TryGet<T>(Guid contentGuid, LoaderOptions loaderOptions, out T content)
        {
            return _defaultContentLoader.TryGet(contentGuid, loaderOptions, out content);
        }
    }
}
