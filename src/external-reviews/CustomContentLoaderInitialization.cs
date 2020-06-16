﻿using System.Linq;
using AdvancedExternalReviews.DraftContentAreaPreview;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomContentLoaderInitialization : IInitializableModule
    {
        private bool _replacementChildren = false;
        private bool _replacementContent = false;

        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            var externalReviewOptions = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            if (externalReviewOptions.ContentReplacement.ReplaceChildren)
            {
                events.LoadingContent += Events_LoadingContent;
                _replacementChildren = true;
            }
            if (externalReviewOptions.ContentReplacement.ReplaceContent)
            {
                events.LoadingChildren += Events_LoadingChildren;
                _replacementContent = true;
            }
        }

        private void Events_LoadingContent(object sender, ContentEventArgs e)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return;
            }

            if (ExternalReview.CustomLoaded.Contains(e.ContentLink.ToString()))
            {
                var preferredCulture = ServiceLocator.Current.GetInstance<LanguageResolver>().GetPreferredCulture();
                var cachedContent = ExternalReview.GetCachedContent(preferredCulture, e.ContentLink);
                if (cachedContent != null)
                {
                    e.ContentLink = cachedContent.ContentLink;
                    e.Content = cachedContent;
                    e.CancelAction = true;
                }

                return;
            }

            ExternalReview.CustomLoaded.Add(e.ContentLink.ToString());

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            var unpublished = reviewsContentLoader.LoadUnpublishedVersion(e.ContentLink);
            if (unpublished == null)
            {
                return;
            }

            var unpublishedCulture = ServiceLocator.Current.GetInstance<LanguageResolver>().GetPreferredCulture();
            var content = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(unpublished);

            ExternalReview.SetCachedLink(unpublishedCulture, content);

            e.ContentLink = unpublished;
            e.Content = content;
        }

        private void Events_LoadingChildren(object sender, ChildrenEventArgs e)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return;
            }

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            e.CancelAction = true;
            e.ChildrenItems = reviewsContentLoader.GetChildrenWithReviews<IContent>(e.ContentLink).ToList();
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            if (_replacementChildren)
            {
                events.LoadingChildren -= Events_LoadingChildren;
            }

            if (_replacementContent)
            {
                events.LoadingContent -= Events_LoadingContent;
            }
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
