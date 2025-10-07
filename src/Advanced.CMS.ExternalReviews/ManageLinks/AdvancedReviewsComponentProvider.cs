using EPiServer.Personalization.VisitorGroups;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ViewComposition;

namespace Advanced.CMS.ExternalReviews.ManageLinks;

[ComponentProvider]
internal class AdvancedReviewsComponentProvider(
    ExternalReviewOptions options,
    IVisitorGroupRepository visitorGroupRepository,
    ModuleTable moduleTable)
    : IComponentProvider
{
    private IEnumerable<IComponentDefinition> _componentDefinitions;
    public int SortOrder => 1000;

    public IEnumerable<IComponentDefinition> GetComponentDefinitions()
    {
        if (_componentDefinitions != null)
        {
            return _componentDefinitions;
        }

        var isAdvancedReviewsModuleAdded = moduleTable.TryGetModule(this.GetType().Assembly, out _);
        var isComponentReady = isAdvancedReviewsModuleAdded && options.IsEnabled;

        if (!isComponentReady)
        {
            return Enumerable.Empty<IComponentDefinition>();
        }

        _componentDefinitions = new [] { new ExternalReviewLinksManageComponent(options, visitorGroupRepository) };
        return _componentDefinitions;
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
