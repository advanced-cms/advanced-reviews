using System.Collections.Concurrent;
using System.Globalization;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.ExternalReviews
{
    [ServiceConfiguration(typeof(ExternalReviewState), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ExternalReviewState
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExternalReviewState(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static object locker = new object();

        public string Token
        {
            get => _httpContextAccessor.HttpContext?.Items["Token"] as string;
            set => _httpContextAccessor.HttpContext.Items["Token"] = value;
        }

        public bool IsEditLink
        {
            get => (string) _httpContextAccessor.HttpContext?.Items["IsEditLink"] == bool.TrueString;
            set => _httpContextAccessor.HttpContext.Items["IsEditLink"] = value.ToString();
        }

        public int? ProjectId
        {
            get => (int?) _httpContextAccessor.HttpContext?.Items["ProjectId"];
            set => _httpContextAccessor.HttpContext.Items["ProjectId"] = value;
        }

        public bool IsInExternalReviewContext => !string.IsNullOrWhiteSpace(Token);
        public bool IsInProjectReviewContext => ProjectId.HasValue;

        public static object _linkLock = new object();

        private ConcurrentDictionary<string, IContent>  GetCachedLinksDictionary()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            var key = "_cachedReviewLinks";
            if (_httpContextAccessor.HttpContext.Items[key] as ConcurrentDictionary<string, IContent> == null)
            {
                lock (_linkLock)
                {
                    if (_httpContextAccessor.HttpContext.Items[key] as ConcurrentDictionary<string, IContent> == null)
                    {
                        _httpContextAccessor.HttpContext.Items[key] = new ConcurrentDictionary<string, IContent>();
                    }
                }
            }

            return _httpContextAccessor.HttpContext?.Items[key] as ConcurrentDictionary<string, IContent>;
        }

        public IContent GetCachedContent(CultureInfo preferredCulture, ContentReference contentLink)
        {
            if (GetCachedLinksDictionary().TryGetValue(preferredCulture.Name + "_" + contentLink.ToReferenceWithoutVersion(), out var result))
            {
                return result;
            }
            return null;
        }

        public void SetCachedLink(CultureInfo preferredCulture, IContent contentLink)
        {
            GetCachedLinksDictionary()[preferredCulture.Name + "_" + contentLink.ContentLink.ToReferenceWithoutVersion()] = contentLink;
        }
    }
}
