using System;
using System.Threading.Tasks;
using Advanced.CMS.ExternalReviews.PinCodeSecurity;
using Advanced.CMS.ExternalReviews.ReviewLinksRepository;
using EPiServer.Core;
using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Advanced.CMS.ExternalReviews
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExternalReviewState _externalReviewState;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;

        public TestMiddleware(RequestDelegate next, ExternalReviewState externalReviewState,
            IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler,
            IExternalReviewLinksRepository externalReviewLinksRepository)
        {
            _next = next;
            _externalReviewState = externalReviewState;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
            _externalReviewLinksRepository = externalReviewLinksRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is PinAuthenticationFailedException)
            {
                context.Response.Redirect("/account/login");
            }

            return Task.CompletedTask;
        }
    }

    public class PinAuthenticationFailedException : Exception
    {
    }

    public static class TestMiddlewareExtensions
    {
        public static IApplicationBuilder UseTestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TestMiddleware>();
        }
    }

    /// <summary>
    /// Partial router used to display readonly version of the page
    /// </summary>
    [ServiceConfiguration(typeof(IPartialRouter))]
    public class PagePreviewPartialRouter : IPartialRouter<PageData, PageData>
    {
        private readonly ProjectContentResolver _projectContentResolver;
        private readonly IExternalLinkPinCodeSecurityHandler _externalLinkPinCodeSecurityHandler;
        private readonly IExternalReviewLinksRepository _externalReviewLinksRepository;
        private readonly ExternalReviewOptions _externalReviewOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExternalReviewState _externalReviewState;

        public PagePreviewPartialRouter(IExternalReviewLinksRepository externalReviewLinksRepository,
            ExternalReviewOptions externalReviewOptions,
            ProjectContentResolver projectContentResolver,
            IExternalLinkPinCodeSecurityHandler externalLinkPinCodeSecurityHandler,
            IHttpContextAccessor httpContextAccessor, ExternalReviewState externalReviewState)
        {
            _externalReviewLinksRepository = externalReviewLinksRepository;
            _externalReviewOptions = externalReviewOptions;
            _projectContentResolver = projectContentResolver;
            _externalLinkPinCodeSecurityHandler = externalLinkPinCodeSecurityHandler;
            _httpContextAccessor = httpContextAccessor;
            _externalReviewState = externalReviewState;
        }

        public PartialRouteData GetPartialVirtualPath(PageData content, UrlGeneratorContext segmentContext)
        {
            return new PartialRouteData();
        }

        public object RoutePartial(PageData content, UrlResolverContext segmentContext)
        {
            if (!_externalReviewOptions.IsEnabled)
            {
                return null;
            }

            var nextSegment = segmentContext.GetNextSegment(segmentContext.RemainingSegments);
            if (nextSegment.Next.IsEmpty)
            {
                return null;
            }

            if (!string.Equals(nextSegment.Next.ToString(), _externalReviewOptions.ContentPreviewUrl,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            nextSegment = segmentContext.GetNextSegment(nextSegment.Remaining);
            var token = nextSegment.Next.ToString();

            var externalReviewLink = _externalReviewLinksRepository.GetContentByToken(token);
            if (!externalReviewLink.IsPreviewableLink())
            {
                return null;
            }

            _externalReviewState.Token = token;
            _externalReviewState.ProjectId = externalReviewLink.ProjectId;
            //TODO: is it used?
            if (externalReviewLink.VisitorGroups != null)
            {
                _httpContextAccessor.HttpContext.Items["ImpersonatedVisitorGroupsById"] =
                    externalReviewLink.VisitorGroups;
            }

            // PIN code security check, if user is not authenticated, then redirect to login page
            if (!_externalLinkPinCodeSecurityHandler.UserHasAccessToLink(externalReviewLink))
            {
                _externalLinkPinCodeSecurityHandler.RedirectToLoginPage(externalReviewLink);
            }

            try
            {
                var page = _projectContentResolver.TryGetProjectPageVersion(externalReviewLink, content,
                    segmentContext.Url.QueryCollection);

                segmentContext.RemainingSegments = nextSegment.Remaining;

                // set ContentLink in DataTokens to make IPageRouteHelper working
                segmentContext.RouteValues[RoutingConstants.ContentLinkKey] = page.ContentLink;

                return page;
            }
            catch
            {
                return null;
            }
        }
    }
}
