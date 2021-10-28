using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace AdvancedExternalReviews
{
    public static class ExternalReview
    {
        public static object locker = new object();

        public static string Token
        {
            get => ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext?.Items["Token"] as string;
            set => ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["Token"] = value;
        }

        public static bool IsEditLink
        {
            get => (string) ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext?.Items["IsEditLink"] == bool.TrueString;
            set => ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["IsEditLink"] = value.ToString();
        }

        public static IList<string> CustomLoaded
        {
            get
            {
                if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext == null)
                {
                    return new List<string>();
                }

                if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["CustomLoaded"] as IList<string> == null)
                {
                    lock (locker)
                    {
                        if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["CustomLoaded"] as IList<string> == null)
                        {
                            ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["CustomLoaded"] = new List<string>();
                        }
                    }
                }

                return ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext?.Items["CustomLoaded"] as IList<string>;
            }
        }

        public static int? ProjectId
        {
            get => (int?) ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext?.Items["ProjectId"];
            set => ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items["ProjectId"] = value;
        }

        public static bool IsInExternalReviewContext => !string.IsNullOrWhiteSpace(Token);
        public static bool IsInProjectReviewContext => ProjectId.HasValue;

        public static object _linkLock = new object();

        private static ConcurrentDictionary<string, IContent>  GetCachedLinksDictionary()
        {
            if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext == null)
            {
                return null;
            }

            var key = "_cachedReviewLinks";
            if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items[key] as ConcurrentDictionary<string, IContent> == null)
            {
                lock (_linkLock)
                {
                    if (ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items[key] as ConcurrentDictionary<string, IContent> == null)
                    {
                        ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext.Items[key] = new ConcurrentDictionary<string, IContent>();
                    }
                }
            }

            return ServiceLocator.Current.GetInstance<HttpContextAccessor>().HttpContext?.Items[key] as ConcurrentDictionary<string, IContent>;
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
