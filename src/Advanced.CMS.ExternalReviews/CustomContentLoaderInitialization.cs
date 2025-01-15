using System;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace Advanced.CMS.ExternalReviews;

[ModuleDependency(typeof(InitializationModule))]
internal class CustomContentLoaderInitialization : IInitializableModule
{
    private static readonly ILogger _log = LogManager.GetLogger(typeof(CustomContentLoaderInitialization));

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

        var unpublished = e.ContentLink.LoadUnpublishedVersion();
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

        if (content is not IVersionable versionable || versionable.HasExpired())
        {
            return;
        }

        externalReviewState.SetCachedLink(content);

        e.ContentLink = unpublished;
        try
        {
            e.Content = content.AllowAccessToEveryone();
        }
        //Issues with AllowAccessToEveryone. This method seems to be failing intermittently, so better to wrap in a naive try/catch and fallback to default ACL
        catch (Exception ex)
        {
            _log.Debug($"Not possible to grant contentLink={content.ContentLink} available to everyone", ex);
            e.Content = content;
        }

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
