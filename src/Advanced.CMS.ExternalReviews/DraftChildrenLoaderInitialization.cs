using System.Globalization;
using EPiServer.Core.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Shell.UI;
using Microsoft.Extensions.Options;

namespace Advanced.CMS.ExternalReviews;

[ModuleDependency(typeof(InitializationModule))]
internal class DraftChildrenLoaderInitialization : IInitializableModule
{
    private static readonly ILogger _log = LogManager.GetLogger(typeof(DraftChildrenLoaderInitialization));

    public void Initialize(InitializationEngine context)
    {
        var events = ServiceLocator.Current.GetInstance<IContentEvents>();
        events.LoadingChildren += Events_LoadingChildren;
    }

    public void Uninitialize(InitializationEngine context)
    {
        var events = ServiceLocator.Current.GetInstance<IContentEvents>();
        events.LoadingChildren -= Events_LoadingChildren;
    }

    private void Events_LoadingChildren(object sender, ChildrenEventArgs e)
    {
        var externalReviewState = ServiceLocator.Current.GetInstance<ExternalReviewState>();
        var options = ServiceLocator.Current.GetInstance<IOptions<ExternalReviewOptions>>();

        if (!options.Value.InterceptGetChildren || !externalReviewState.IsInExternalReviewContext)
        {
            return;
        }

        try
        {
            var draftChildrenLoader = ServiceLocator.Current.GetInstance<DraftChildrenLoader>();

            var language = !string.IsNullOrEmpty(externalReviewState.PreferredLanguage)
                ? new CultureInfo(externalReviewState.PreferredLanguage)
                : CultureInfo.CurrentCulture;

            var childrenWithReviews = draftChildrenLoader.GetChildrenWithReviews(e.ContentLink, language);

            e.ChildrenItems = childrenWithReviews.ToList();
            e.CancelAction = true;
        }
        catch (Exception ex)
        {
            _log.Error($"Error loading children with reviews for content {e.ContentLink}", ex);
        }
    }
}
