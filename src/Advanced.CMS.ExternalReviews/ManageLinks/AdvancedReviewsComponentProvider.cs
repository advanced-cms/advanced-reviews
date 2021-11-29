using System.Collections.Generic;
using System.Linq;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ViewComposition;

namespace Advanced.CMS.ExternalReviews.ManageLinks
{
    [ComponentProvider]
    public class AdvancedReviewsComponentProvider : IComponentProvider
    {
        private readonly ExternalReviewOptions _options;
        private readonly IVisitorGroupRepository _visitorGroupRepository;
        private readonly ModuleTable _moduleTable;
        public int SortOrder => 1000;

        public AdvancedReviewsComponentProvider(ExternalReviewOptions options, IVisitorGroupRepository visitorGroupRepository, ModuleTable moduleTable)
        {
            _options = options;
            _visitorGroupRepository = visitorGroupRepository;
            _moduleTable = moduleTable;
        }

        public IEnumerable<IComponentDefinition> GetComponentDefinitions()
        {
            var isAdvancedReviewsModuleAdded = _moduleTable.TryGetModule(this.GetType().Assembly, out _);
            var isComponentReady = isAdvancedReviewsModuleAdded && _options.IsEnabled;

            if (!isComponentReady)
            {
                return Enumerable.Empty<IComponentDefinition>();
            }

            return new [] { new ExternalReviewLinksManageComponent(_options, _visitorGroupRepository) };
        }

        public IComponent CreateComponent(IComponentDefinition definition)
        {
            if (GetComponentDefinitions().Contains(definition))
            {
                return definition.CreateComponent();
            }
            return null;
        }
    }
}
