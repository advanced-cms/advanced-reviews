using System.Web;

namespace AdvancedExternalReviews
{
    public static class ExternalReview
    {
        public static object locker = new object();

        public static string Token
        {
            get => HttpContext.Current?.Items["Token"] as string;
            set => HttpContext.Current.Items["Token"] = value;
        }

        public static bool IsEditLink
        {
            get => (string) HttpContext.Current?.Items["IsEditLink"] == bool.TrueString;
            set => HttpContext.Current.Items["IsEditLink"] = value.ToString();
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
