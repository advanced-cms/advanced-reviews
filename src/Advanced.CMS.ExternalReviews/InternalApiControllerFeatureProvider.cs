using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Advanced.CMS.ExternalReviews;

internal sealed class InternalApiControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeInfo.IsClass || typeInfo.IsAbstract || typeInfo.IsNested || typeInfo.ContainsGenericParameters)
        {
            return false;
        }

        // Only controllers in EPiServer. dlls
        if (typeInfo.Assembly.FullName == null || !typeInfo.Assembly.FullName.StartsWith("Advanced.CMS.", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return typeInfo.IsAssignableTo(typeof(Controller));
    }
}
