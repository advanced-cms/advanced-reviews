using System.Collections.Generic;
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

        public static IList<string> CustomLoaded
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return new List<string>();
                }

                if (HttpContext.Current.Items["CustomLoaded"] as IList<string> == null)
                {
                    lock (locker)
                    {
                        if (HttpContext.Current.Items["CustomLoaded"] as IList<string> == null)
                        {
                            HttpContext.Current.Items["CustomLoaded"] = new List<string>();
                        }
                    }
                }

                return HttpContext.Current?.Items["CustomLoaded"] as IList<string>;
            }
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
