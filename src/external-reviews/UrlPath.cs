using System.Linq;
using EPiServer;

namespace AdvancedExternalReviews
{
    public static class UrlPath
    {
        public static string Combine(string url, params string[] pathFragments)
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

        public static string EnsureStartsWithSlash(string url)
        {
            url = url.TrimEnd('/', '\\');
            return $"/{url}";
        }
    }
}
