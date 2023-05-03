using Advanced.CMS.ExternalReviews.DraftContentAreaPreview;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
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
                var cachedContent = externalReviewState.GetCachedContent(e.ContentLink);
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

            var externalReviewLinksRepository = ServiceLocator.Current.GetInstance<IExternalReviewLinksRepository>();
            var externalReviewLink = externalReviewLinksRepository.GetContentByToken(externalReviewState.Token);
            if (!externalReviewLink.IsPreviewableLink())
            {
                return;
            }

            var content = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(unpublished);

            if (content is not IVersionable versionable || reviewsContentLoader.HasExpired(versionable))
            {
                return;
            }

            externalReviewState.SetCachedLink(content);

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
