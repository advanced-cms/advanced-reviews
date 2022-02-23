using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TestSite
{
    public class SkipAntiforgeryFilterProvider : IFilterProvider
    {
        public int Order => int.MaxValue;

        public void OnProvidersExecuted(FilterProviderContext context)
        {
        }

        public void OnProvidersExecuting(FilterProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var FilterDescriptor = new FilterDescriptor(SkipAntiforgeryPolicy.Instance, FilterScope.Last);
            var filterItem = new FilterItem(FilterDescriptor, SkipAntiforgeryPolicy.Instance);
            context.Results.Add(filterItem);
        }

        class SkipAntiforgeryPolicy : IAntiforgeryPolicy, IAsyncAuthorizationFilter
        {
            public static readonly SkipAntiforgeryPolicy Instance = new SkipAntiforgeryPolicy();

            public Task OnAuthorizationAsync(AuthorizationFilterContext context) => Task.CompletedTask;
        }
    }
}
