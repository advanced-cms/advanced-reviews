using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Alloy.Sample.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RegisterFirstAdminWithLocalRequestAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AdministratorRegistrationPageMiddleware.IsEnabled == false)
            {
                context.Result = new NotFoundResult();
                return;
            }
        }
    }
}
