using System.Security.Principal;
using EPiServer.Security;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview;

internal class ContentAccessEvaluatorDecorator(
    IContentAccessEvaluator defaultContentAccessEvaluator,
    ExternalReviewState externalReviewState)
    : IContentAccessEvaluator
{
    public bool HasAccess(IContent content, IPrincipal principal, AccessLevel access)
    {
        if (externalReviewState.IsInExternalReviewContext)
        {
            return true;
        }

        return defaultContentAccessEvaluator.HasAccess(content, principal, access);
    }

    public AccessLevel GetAccessLevel(IContent content, IPrincipal principal)
    {
        if (externalReviewState.IsInExternalReviewContext)
        {
            return AccessLevel.Read;
        }

        return defaultContentAccessEvaluator.GetAccessLevel(content, principal);
    }
}
