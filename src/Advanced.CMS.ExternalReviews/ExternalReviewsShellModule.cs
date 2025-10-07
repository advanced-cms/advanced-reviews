using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace Advanced.CMS.ExternalReviews;

public class ExternalReviewsShellModule(string name, string routeBasePath, string resourceBasePath)
    : ShellModule(name, routeBasePath, resourceBasePath)
{
    /// <inheritdoc />
    public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
    {
        var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
        return new AdvancedReviewsModuleViewModel(this, clientResourceService, options);
    }
}
