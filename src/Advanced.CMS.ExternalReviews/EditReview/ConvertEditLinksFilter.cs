namespace Advanced.CMS.ExternalReviews.EditReview
{
    //TODO NETCORE: // public class ConvertEditLinksFilter : ActionFilterAttribute
    // {
    //     private readonly ExternalReviewOptions _externalReviewOptions;
    //     private readonly IUrlResolver _urlResolver;
    //
    //     public ConvertEditLinksFilter()
    //     {
    //         _externalReviewOptions = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
    //         _urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();
    //     }
    //
    //     /// <summary>
    //     /// Use regex to find all images src attributes and replace them with viewmode URLs
    //     /// </summary>
    //     /// <param name="html"></param>
    //     /// <returns></returns>
    //     private string ReplaceImages(string html)
    //     {
    //         const string pattern = "<img.*?src=[\"'](.+?)[\"'].*?>";
    //
    //         html = Regex.Replace(html, pattern, x =>
    //         {
    //             if (x.Groups.Count == 1)
    //             {
    //                 return x.Value;
    //             }
    //
    //             var imgSrc = x.Groups[1];
    //             var editUrl = imgSrc.Value;
    //             var content = _urlResolver.Route(new UrlBuilder(editUrl), ContextMode.Edit);
    //             if (content == null)
    //             {
    //                 return x.Value;
    //             }
    //             var viewUrl = _urlResolver.GetUrl(content.ContentLink, content.LanguageBranch(), new UrlResolverArguments()
    //             {
    //                 ContextMode = ContextMode.Default
    //             });
    //
    //             var groupStart = imgSrc.Index - x.Index;
    //             var groupEnd = groupStart + imgSrc.Length;
    //
    //             return x.Value.Substring(0, groupStart - 1) + viewUrl + x.Value.Substring(groupEnd + 1);
    //         });
    //
    //         return html;
    //     }
    //
    //     public override void OnActionExecuting(ActionExecutingContext filterContext)
    //     {
    //         if (!ExternalReview.IsInExternalReviewContext)
    //         {
    //             return;
    //         }
    //
    //         if (!ExternalReview.IsEditLink)
    //         {
    //             return;
    //         }
    //
    //         // Set context to Edit to get the data-epi attributes render. It has to be done here, after the routing is already done and before
    //         // controller render the HTML
    //         filterContext..HttpContext.Request.RequestContext.SetContextMode(ContextMode.Edit);
    //     }
    //
    //     public override void OnResultExecuting(ResultExecutingContext filterContext)
    //     {
    //         if (!ExternalReview.IsInExternalReviewContext)
    //         {
    //             return;
    //         }
    //
    //         if (!PageEditing.PageIsInEditMode)
    //         {
    //             return;
    //         }
    //
    //         // check if URL contains external URL path
    //         if (filterContext.HttpContext.Request.Url == null || filterContext.HttpContext.Request.Url.ToString()
    //                 .IndexOf(_externalReviewOptions.ContentIframeEditUrlSegment, StringComparison.Ordinal) < 0)
    //         {
    //             return;
    //         }
    //
    //         if (filterContext.HttpContext.Response.ContentType != "text/html")
    //         {
    //             return;
    //         }
    //
    //         // check if filters are enabled for the request
    //         if (filterContext.IsChildAction)
    //         {
    //             return;
    //         }
    //
    //         if (filterContext.Result is ViewResultBase)
    //         {
    //             var response = filterContext.HttpContext.Response;
    //             response.Output = new BufferedTextWriter(response.Output, ReplaceHtml);
    //         }
    //     }
    //
    //     private string RemoveEPiHtml(string html)
    //     {
    //         html = Regex.Replace(html, "(<script.*epi-cms/communicationInjector.js.*</script>)", "", RegexOptions.IgnoreCase);
    //         html = Regex.Replace(html, "(<link.*epi-cms/epiEditMode.css.* />)", "", RegexOptions.IgnoreCase);
    //         return html;
    //     }
    //
    //     private string ReplaceHtml(string html)
    //     {
    //         html = ReplaceImages(html);
    //         html = RemoveEPiHtml(html);
    //         return html;
    //     }
    //
    //     public override void OnResultExecuted(ResultExecutedContext filterContext)
    //     {
    //         if (filterContext.HttpContext.Response.Output is BufferedTextWriter bufferedTextWriter)
    //         {
    //             try
    //             {
    //                 bufferedTextWriter.Flush(filterContext.Exception == null);
    //             }
    //             finally
    //             {
    //                 filterContext.HttpContext.Response.Output = bufferedTextWriter.InnerTextWriter;
    //             }
    //         }
    //     }
    // }
    //
    // public class BufferedTextWriter: TextWriter
    // {
    //     private readonly StringBuilder stringBuilder = new StringBuilder();
    //     public readonly TextWriter InnerTextWriter;
    //
    //     private readonly Func<string, string> textRewriteFunction;
    //
    //     public BufferedTextWriter(TextWriter innerTextWriter, Func<string, string> textRewriteFunction)
    //     {
    //         InnerTextWriter = innerTextWriter ?? throw new ArgumentNullException(nameof(innerTextWriter));
    //         this.textRewriteFunction = textRewriteFunction;
    //     }
    //
    //     public override Encoding Encoding => InnerTextWriter.Encoding;
    //
    //     public override void Write(char value)
    //     {
    //         stringBuilder.Append(value);
    //     }
    //
    //     public void Flush(bool rewriteText)
    //     {
    //         var bufferedText = stringBuilder.ToString();
    //
    //         if (rewriteText && textRewriteFunction != null)
    //         {
    //             bufferedText = textRewriteFunction.Invoke(bufferedText);
    //         }
    //
    //         InnerTextWriter.Write(bufferedText);
    //         InnerTextWriter.Flush();
    //     }
    // }
}
