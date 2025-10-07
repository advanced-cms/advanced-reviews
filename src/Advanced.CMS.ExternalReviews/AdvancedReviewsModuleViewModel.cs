using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Modules;

namespace Advanced.CMS.ExternalReviews;

internal class AdvancedReviewsModuleViewModel(
    ShellModule shellModule,
    IClientResourceService clientResourceService,
    ExternalReviewOptions options)
    : ModuleViewModel(shellModule, clientResourceService)
{
    public string Language { get; set; }
    public string AvatarUrl { get; set; }
    public ExternalReviewOptions Options { get; } = options;
}
