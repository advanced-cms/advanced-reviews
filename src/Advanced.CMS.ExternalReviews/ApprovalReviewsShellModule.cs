using Advanced.CMS.ApprovalReviews;
using EPiServer.Framework.Web.Resources;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Profile.Internal;
using Microsoft.Extensions.Options;

namespace Advanced.CMS.ExternalReviews;

public class ApprovalReviewsShellModule(string name, string routeBasePath, string resourceBasePath)
    : ShellModule(name, routeBasePath, resourceBasePath)
{
    /// <inheritdoc />
    public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
    {
        var options = ServiceLocator.Current.GetInstance<IOptions<ExternalReviewOptions>>().Value;
        var reviewUrlGenerator = ServiceLocator.Current.GetInstance<ReviewUrlGenerator>();
        var principal = ServiceLocator.Current.GetInstance<IPrincipalAccessor>();
        var currentUiCulture = ServiceLocator.Current.GetInstance<ICurrentUiCulture>();
        var model = new AdvancedReviewsModuleViewModel(this, clientResourceService, options)
        {
            Language = currentUiCulture.Get(principal.Principal.Identity.Name).Name,
            AvatarUrl = reviewUrlGenerator.AvatarUrl
        };
        return model;
    }
}
