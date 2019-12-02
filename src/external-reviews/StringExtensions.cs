namespace AdvancedExternalReviews
{
    public static class UrlPath
    {
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
