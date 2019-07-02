using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews.ReviewLinksRepository
{
    [ServiceConfiguration(typeof(ExternalReviewLinkBuilder))]
    public class ExternalReviewLinkBuilder
    {
        private readonly UrlResolver _urlResolver;
        private readonly ExternalReviewOptions _options;

        public ExternalReviewLinkBuilder(UrlResolver urlResolver, ExternalReviewOptions options)
        {
            _urlResolver = urlResolver;
            _options = options;
        }

        public ExternalReviewLink FromExternalReview(ExternalReviewLinkDds externalReviewLinkDds)
        {
            var startPageUrl = _urlResolver.GetUrl(ContentReference.StartPage);
            // the preview url has to be language specific as it's handled entirely by the EPiServer partial router
            // the edit url is just a pure aspnet.mvc controller, handled outside EPiServer
            var previewUrl = startPageUrl + _options.ContentPreviewUrl;

            return new ExternalReviewLink
            {
                ContentLink = externalReviewLinkDds.ContentLink,
                IsEditable = externalReviewLinkDds.IsEditable,
                Token = externalReviewLinkDds.Token,
                ValidTo = externalReviewLinkDds.ValidTo,
                LinkUrl = (externalReviewLinkDds.IsEditable ? "/" + _options.ReviewsUrl: previewUrl) + "/" + externalReviewLinkDds.Token //TODO: externalReviews URL
            };
        }
    }
}
