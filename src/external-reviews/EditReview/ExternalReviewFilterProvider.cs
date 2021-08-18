using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdvancedExternalReviews.EditReview
{
    public class ExternalReviewFilterProvider : IFilterProvider
    {
        // private readonly FilterProviderCollection _filterProviders;
        // private readonly Type _authorizeContent = typeof(AuthorizeContentAttribute);

        // public ExternalReviewFilterProvider(IList<IFilterProvider> filters)
        // {
        //     _filterProviders = new FilterProviderCollection(filters);
        // }

        // public IEnumerable<Filter> GetFilters(ControllerContext controllerContext,
        //     ActionDescriptor actionDescriptor)
        // {
        //     var filters = _filterProviders.GetFilters(controllerContext, actionDescriptor);
        //     return ExternalReview.IsInExternalReviewContext ? filters.Where(x => x.Instance.GetType() != _authorizeContent) : filters;
        // }

        public void OnProvidersExecuting(FilterProviderContext context)
        {
            throw new NotImplementedException();
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
            throw new NotImplementedException();
        }

        public int Order { get; }
    }
}
