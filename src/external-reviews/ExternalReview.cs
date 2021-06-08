using System.Collections.Concurrent;
using System.Globalization;
using System.Web;
using EPiServer.Core;

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

        public static object _linkLock = new object();

        private static ConcurrentDictionary<string, IContent>  GetCachedLinksDictionary()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }

            var key = "_cachedReviewLinks";
            if (HttpContext.Current.Items[key] as ConcurrentDictionary<string, IContent> == null)
            {
                lock (_linkLock)
                {
                    if (HttpContext.Current.Items[key] as ConcurrentDictionary<string, IContent> == null)
                    {
                        HttpContext.Current.Items[key] = new ConcurrentDictionary<string, IContent>();
                    }
                }
            }

            return HttpContext.Current?.Items[key] as ConcurrentDictionary<string, IContent>;
        }

        public static IContent GetCachedContent(CultureInfo preferredCulture, ContentReference contentLink)
        {
            if (GetCachedLinksDictionary().TryGetValue(preferredCulture.Name + "_" + contentLink.ToReferenceWithoutVersion(), out var result))
            {
                return result;
            }
            return null;
        }

        public static void SetCachedLink(CultureInfo preferredCulture, IContent contentLink)
        {
            GetCachedLinksDictionary()[preferredCulture.Name + "_" + contentLink.ContentLink.ToReferenceWithoutVersion()] = contentLink;
        }
    }
}
