using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.CMS.IntegrationTests
{
    /// <summary>
    /// Enables faking a user in the HttpContext by passing request params "username" and "roles" (comma separated)
    /// Example: http://somesite/endpoint?username=Bob&roles=WebAdmins,WebEditors
    /// </summary>
    public class FakeUserMiddleware(RequestDelegate next)
    {
        private const string UsernameKey = "username";
        private const string RolesKey = "roles";

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Query.TryGetValue(UsernameKey, out var username))
            {
                httpContext.User = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity(username), ParseRolesFromQuery(httpContext.Request.Query)));
                var claimTransforms = httpContext.RequestServices.GetServices<IClaimsTransformation>();
                foreach (var transform in claimTransforms)
                {
                    httpContext.User = await transform.TransformAsync(httpContext.User);
                }
            }

            await next(httpContext);
        }

        private static string[] ParseRolesFromQuery(IQueryCollection query)
        {
            if (query.TryGetValue(RolesKey, out var roles))
            {
                return roles.ToString()?.Split(',') ?? Array.Empty<string>();
            }

            return Array.Empty<string>();
        }
    }
}
