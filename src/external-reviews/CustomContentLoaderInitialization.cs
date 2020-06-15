using System.Linq;
using AdvancedExternalReviews.DraftContentAreaPreview;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AdvancedExternalReviews
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomContentLoaderInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            var externalReviewOptions = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            if (externalReviewOptions.ReplaceChildren)
            {
                events.LoadingChildren += Events_LoadingChildren;
                events.LoadingContent += Events_LoadingContent;
            }
        }

        private void Events_LoadingContent(object sender, ContentEventArgs e)
        {
            if (!ExternalReview.IsInExternalReviewContext || ExternalReview.CustomLoaded.Contains(e.ContentLink.ToString()))
            {
                return;
            }

            ExternalReview.CustomLoaded.Add(e.ContentLink.ToString());

            var reviewsContentLoader = ServiceLocator.Current.GetInstance<ReviewsContentLoader>();

            var unpublished = reviewsContentLoader.LoadUnpublishedVersion(e.ContentLink);
            if (unpublished == null)
            {
                return;
            }

            e.ContentLink = unpublished;
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
            events.LoadingChildren -= Events_LoadingChildren;
            events.LoadingContent -= Events_LoadingContent;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
