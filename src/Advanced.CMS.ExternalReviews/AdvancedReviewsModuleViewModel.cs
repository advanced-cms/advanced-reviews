using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Modules;

namespace Advanced.CMS.ExternalReviews;

internal class AdvancedReviewsModuleViewModel : ModuleViewModel
{
    public AdvancedReviewsModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService, ExternalReviewOptions options) :
        base(shellModule, clientResourceService)
    {
        Options = options;
    }

    public string Language { get; set; }
    public string AvatarUrl { get; set; }
    public ExternalReviewOptions Options { get; }
}
