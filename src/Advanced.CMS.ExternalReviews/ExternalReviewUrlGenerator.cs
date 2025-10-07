using Advanced.CMS.ExternalReviews.EditReview;
using EPiServer.Shell;

namespace Advanced.CMS.ExternalReviews;

internal class ExternalReviewUrlGenerator(ExternalReviewState externalReviewState)
{
    public string ReviewsUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.Index)}");
    public string AddPinUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.AddPin)}");
    public string RemovePinUrl => Paths.ToResource("advanced-cms-external-reviews", $"PageEdit/{nameof(PageEditController.RemovePin)}");

    public string GetProxiedImageUrl(ContentReference contentLink)
    {
        return $"/ImageProxy/{externalReviewState.Token}/{contentLink}";
    }
}
