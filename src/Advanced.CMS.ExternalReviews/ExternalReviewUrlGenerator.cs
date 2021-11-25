using Advanced.CMS.ExternalReviews.EditReview;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Advanced.CMS.ExternalReviews
{
    [ServiceConfiguration]
    public class ExternalReviewUrlGenerator
    {
        private readonly ExternalReviewState _externalReviewState;
        public string ReviewsUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.Index)}");
        public string AddPinUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.AddPin)}");
        public string RemovePinUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.RemovePin)}");

        public ExternalReviewUrlGenerator(ExternalReviewState externalReviewState)
        {
            _externalReviewState = externalReviewState;
        }

        public string GetProxiedImageUrl(ContentReference contentLink)
        {
            return $"/ImageProxy?token={_externalReviewState.Token}&contentLink={contentLink}";
        }
    }
}
