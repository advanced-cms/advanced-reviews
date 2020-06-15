using System;
using System.Linq;
using AdvancedExternalReviews;
using AdvancedExternalReviews.DraftContentAreaPreview;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;

namespace AlloyTemplates.Models.Pages
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DisplayModesInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            if (ServiceLocator.Current.GetInstance<ExternalReviewOptions>().ReplaceChildren)
            {
                events.LoadedChildren += Events_LoadedChildren;
            }
        }

        private void Events_LoadedChildren(object sender, ChildrenEventArgs e)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return;
            }

            e.CancelAction = true;
            e.ChildrenItems = ServiceLocator.Current.GetInstance<ReviewsContentLoader>().GetChildrenWithReviews<IContent>(e.ContentLink).ToList();
        }

        public void Uninitialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.LoadedChildren += Events_LoadedChildren;
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
