using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Editor;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace AdvancedExternalReviews.EditReview
{
    public class ConvertEditLinksFilter: ActionFilterAttribute
    {
        private readonly ExternalReviewOptions _externalReviewOptions;

        public ConvertEditLinksFilter()
        {
            _externalReviewOptions = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
        }

        public override void OnActionExecuting
            (ActionExecutingContext filterContext)
        {
            if (!ExternalReview.IsInExternalReviewContext)
            {
                return;
            }

            if (!PageEditing.PageIsInEditMode)
            {
                return;
            }

            // check if URL contains external URL path
            if (filterContext.HttpContext.Request.Url == null || filterContext.HttpContext.Request.Url.ToString()
                    .IndexOf(_externalReviewOptions.ContentIframeEditUrlSegment, StringComparison.Ordinal) < 0)
            {
                return;
            }

            var response = filterContext.HttpContext.Response;

            if (response.ContentType != "text/html")
            {
                return;
            }

            // check if filters are enabled for the request
            if (filterContext.IsChildAction)
            {
                return;
            }

            var originalFilter =
                filterContext.HttpContext.Response.Filter;
            filterContext.HttpContext.Response.Filter =
                new LinksFilter(originalFilter);
        }

        public class LinksFilter : MemoryStream
        {
            private readonly Stream _responseStream;
            private readonly UrlResolver _urlResolver;

            public LinksFilter(Stream stream)
            {
                _urlResolver = UrlResolver.Current;
                _responseStream = stream;
            }

            public override void Write(byte[] buffer,
                int offset, int count)
            {
                var html = Encoding.UTF8.GetString(buffer);
                html = ReplaceImages(html);
                buffer = Encoding.UTF8.GetBytes(html);
                _responseStream.Write(buffer, offset, buffer.Length);
            }
            
            /// <summary>
            /// Use regex to find all images src attributes and replace them with viewmode URLs
            /// </summary>
            /// <param name="html"></param>
            /// <returns></returns>
            private string ReplaceImages(string html)
            {
                const string pattern = "<img.*?src=[\"'](.+?)[\"'].*?>";

                html = Regex.Replace(html, pattern, x =>
                {
                    if (x.Groups.Count == 1)
                    {
                        return x.Value;
                    }

                    var imgSrc = x.Groups[1];
                    var editUrl = imgSrc.Value;
                    var content = _urlResolver.Route(new UrlBuilder(editUrl), ContextMode.Edit);
                    if (content == null)
                    {
                        return x.Value;
                    }
                    var viewUrl = _urlResolver.GetUrl(content.ContentLink, content.LanguageBranch(), new VirtualPathArguments
                    {
                        ContextMode = ContextMode.Default
                    });

                    var groupStart = imgSrc.Index - x.Index;
                    var groupEnd = groupStart + imgSrc.Length;

                    return x.Value.Substring(0, groupStart - 1) + viewUrl + x.Value.Substring(groupEnd + 1);
                });


                return html;
            }
        }
    }
}
