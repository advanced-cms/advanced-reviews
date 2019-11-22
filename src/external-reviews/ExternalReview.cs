using System.Web;

namespace AdvancedExternalReviews
{
    public static class ExternalReview
    {
        public static string Token
        {
            get => HttpContext.Current?.Items["Token"] as string;
            set => HttpContext.Current.Items["Token"] = value;
        }

        public static int? ProjectId
        {
            get => (int?) HttpContext.Current?.Items["ProjectId"];
            set => HttpContext.Current.Items["ProjectId"] = value;
        }

        public static bool IsInExternalReviewContext => !string.IsNullOrWhiteSpace(Token);
        public static bool IsInProjectReviewContext => ProjectId.HasValue;
    }
}
