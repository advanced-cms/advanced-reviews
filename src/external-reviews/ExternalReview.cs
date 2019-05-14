using System.Web;

namespace AdvancedExternalReviews
{
    public static class ExternalReview
    {
        public static bool IsInExternalReviewContext
        {
            //TODO: check token
            get
            {
                var isInExternalReviewMode = HttpContext.Current.Items["IsInExternalRviewMode"];
                return (isInExternalReviewMode is bool b) && b;
            }
            set => HttpContext.Current.Items["IsInExternalRviewMode"] = value;
        }
    }
}
