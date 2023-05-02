using Advanced.CMS.ExternalReviews.DraftContentAreaPreview;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews
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
            var externalReviewState = ServiceLocator.Current.GetInstance<ExternalReviewState>();

            if (!externalReviewState.IsInExternalReviewContext)
            {
                return;
            }

            if (externalReviewState.CustomLoaded.Contains(e.ContentLink.ToString()))
            {
                var preferredCulture = ServiceLocator.Current.GetInstance<LanguageResolver>().GetPreferredCulture();
                var cachedContent = externalReviewState.GetCachedContent(preferredCulture, e.ContentLink);
                if (cachedContent != null)
                {
                    e.ContentLink = cachedContent.ContentLink;
                    e.Content = cachedContent;
                    e.CancelAction = true;
                }

                return;
            }

            externalReviewState.CustomLoaded.Add(e.ContentLink.ToString());

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            var unpublished = reviewsContentLoader.LoadUnpublishedVersion(e.ContentLink);
            if (unpublished == null)
            {
                return;
            }

            var unpublishedCulture = ServiceLocator.Current.GetInstance<LanguageResolver>().GetPreferredCulture();
            var content = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(unpublished);

            if (!(content is IVersionable versionable) || reviewsContentLoader.HasExpired(versionable))
            {
                return;
            }

            externalReviewState.SetCachedLink(unpublishedCulture, content);

            e.ContentLink = unpublished;
            e.Content = content.AllowAccessToEveryone();
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
