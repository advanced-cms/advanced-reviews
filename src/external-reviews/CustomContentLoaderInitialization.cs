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
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.LoadingContent += Events_LoadingContent;
        }

        private void Events_LoadingContent(object sender, ContentEventArgs e)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return;
            }

            var languageResolver = ServiceLocator.Current.GetInstance<LanguageResolver>();
            var preferredCulture = languageResolver.GetPreferredCulture();
            var cachedContent = ExternalReview.GetCachedContent(preferredCulture, e.ContentLink);
            if (cachedContent != null)
            {
                e.ContentLink = cachedContent.ContentLink;
                e.Content = cachedContent;
                e.CancelAction = true;
                return;
            }

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            var unpublished = reviewsContentLoader.LoadUnpublishedVersion(e.ContentLink);
            if (unpublished == null)
            {
                return;
            }

            var unpublishedCulture = languageResolver.GetPreferredCulture();
            var providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();
            var provider = providerManager.GetProvider(e.ContentLink.ProviderName);
            var content = provider.Load(unpublished, new LanguageSelector(unpublishedCulture.Name));

            if (!(content is IVersionable versionable) || reviewsContentLoader.HasExpired(versionable))
            {
                return;
            }

            ExternalReview.SetCachedLink(unpublishedCulture, content);

            e.ContentLink = unpublished;
            e.Content = content;
            e.CancelAction = true;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.LoadingContent -= Events_LoadingContent;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
