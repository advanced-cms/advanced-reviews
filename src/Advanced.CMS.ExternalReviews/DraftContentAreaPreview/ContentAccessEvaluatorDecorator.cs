using System.Security.Principal;
using EPiServer.Core;
using EPiServer.Security;

namespace Advanced.CMS.ExternalReviews.DraftContentAreaPreview
{
    public class ContentAccessEvaluatorDecorator : IContentAccessEvaluator
    {
        private readonly IContentAccessEvaluator _defaultContentAccessEvaluator;
        private readonly ExternalReviewState _externalReviewState;

        public ContentAccessEvaluatorDecorator(IContentAccessEvaluator defaultContentAccessEvaluator, ExternalReviewState externalReviewState)
        {
            _defaultContentAccessEvaluator = defaultContentAccessEvaluator;
            _externalReviewState = externalReviewState;
        }

        public bool HasAccess(IContent content, IPrincipal principal, AccessLevel access)
        {
            if (_externalReviewState.IsInExternalReviewContext)
            {
                return true;
            }

            return _defaultContentAccessEvaluator.HasAccess(content, principal, access);
        }

        public AccessLevel GetAccessLevel(IContent content, IPrincipal principal)
        {
            if (_externalReviewState.IsInExternalReviewContext)
            {
                return AccessLevel.Read;
            }

            return _defaultContentAccessEvaluator.GetAccessLevel(content, principal);
        }
    }
}
