using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    [ServiceConfiguration(typeof(ExternalReviewLinkBuilder))]
    public class ExternalReviewLinkBuilder
    {
        private readonly UrlResolver _urlResolver;
        private readonly IContentLoader _contentLoader;
        private readonly ExternalReviewOptions _options;

        public ExternalReviewLinkBuilder(UrlResolver urlResolver, ExternalReviewOptions options, IContentLoader contentLoader)
        {
            _urlResolver = urlResolver;
            _options = options;
            _contentLoader = contentLoader;
        }

        public ExternalReviewLink FromExternalReview(ExternalReviewLinkDds externalReviewLinkDds)
        {
            ContentReference contentReference = ContentReference.StartPage;
            if (externalReviewLinkDds.ContentLink != null)
            {
                // if the page has been published before we can generate a link like /alloy-plan/${_options.ContentPreviewUrl}
                // however if the page has never been published then we have to "proxy" it through the StartPage so that the
                // AuthorizationFilter does not block it
                var content = _contentLoader.Get<IContent>(externalReviewLinkDds.ContentLink.ToReferenceWithoutVersion());
                contentReference = content.IsPublished()
                    ? externalReviewLinkDds.ContentLink
                    : ContentReference.StartPage;
            }

            var url = _urlResolver.GetUrl(contentReference);
            // the preview url has to be language specific as it's handled entirely by the EPiServer partial router
            // the edit url is just a pure aspnet.mvc controller, handled outside EPiServer
            var previewUrl = url + _options.ContentPreviewUrl;

            return new ExternalReviewLink
            {
                ContentLink = externalReviewLinkDds.ContentLink,
                IsEditable = externalReviewLinkDds.IsEditable,
                ProjectId = externalReviewLinkDds.ProjectId,
                Token = externalReviewLinkDds.Token,
                ValidTo = externalReviewLinkDds.ValidTo,
                LinkUrl = (externalReviewLinkDds.IsEditable ? "/" + _options.ReviewsUrl: previewUrl) + "/" + externalReviewLinkDds.Token, //TODO: externalReviews URL
                PinCode = externalReviewLinkDds.PinCode
            };
        }
    }
}
