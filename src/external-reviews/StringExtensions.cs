using System.Linq;
using EPiServer;

namespace AdvancedExternalReviews
{
    public static class UrlPath
    {
        public static string AddFragment(string url, string pathFragment)
        {
            return AddFragments(url, pathFragment);
        }

        public static string AddFragments(string url, params string[] pathFragments)
        {
            if (pathFragments.Length == 0)
            {
                return url;
            }


            var urlBuilder = new UrlBuilder(url);
            urlBuilder.Path =
                string.Join(
                    "/",
                    new[] { urlBuilder.Path.TrimEnd('/', '\\') }.
                        Concat(pathFragments.
                            Where(x => x.Length > 0).
                            Select(x => x.TrimStart('/', '\\'))));

            return urlBuilder.ToString();
        }

        public static string Combine(string url1, string url2)
        {
            if (url1.Length == 0) {
                return url2;
            }

            if (url2.Length == 0) {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return $"{url1}/{url2}";
        }

        public static string EnsureStartsWithSlash(string url)
        {
            url = url.TrimEnd('/', '\\');
            return $"/{url}";
        }
    }
}
