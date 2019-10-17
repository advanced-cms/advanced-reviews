using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Web.Mvc;

namespace AdvancedExternalReviews.EditReview
{
    public class ExternalReviewFilterProvider : IFilterProvider
    {
        private readonly FilterProviderCollection _filterProviders;
        private readonly Type _authorizeContent = typeof(AuthorizeContentAttribute);

        public ExternalReviewFilterProvider(IList<IFilterProvider> filters)
        {
            _filterProviders = new FilterProviderCollection(filters);
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var filters = _filterProviders.GetFilters(controllerContext, actionDescriptor);
            return ExternalReview.IsInExternalReviewContext ? filters.Where(x => x.Instance.GetType() != _authorizeContent) : filters;
        }
    }
}
