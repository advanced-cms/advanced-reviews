using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Advanced.CMS.AdvancedReviews.IntegrationTests.PinSecurity;

public static class ObjectExtensions
{
    public static IDictionary<string, string> AsDictionary<T>(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
    {
        return (IDictionary<string, string>)source.GetType().GetProperties(bindingAttr).ToDictionary
        (
            propInfo => propInfo.Name,
            propInfo => (T)propInfo.GetValue(source, null));

    }
}
