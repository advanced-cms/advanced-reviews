using System.Linq;
using EPiServer.Core;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace AdvancedExternalReviews.BlocksPreview
{
    public class BlocksTemplateCoordinator
    {
        public static void OnTemplateResolved(object sender, TemplateResolverEventArgs args)
        {
            var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
            if (!options.IsEnabled && !options.IsPublicPreviewEnabled)
            {
                return;
            }

            if (args.ItemToRender is BlockData)
            {
                if (args.RequestedCategory == TemplateTypeCategories.MvcController ||
                    args.RequestedCategory == TemplateTypeCategories.Page)
                {
                    if (!ExternalReview.IsInExternalReviewContext)
                    {
                        return;
                    }

                    var blockPreviewController =
                        args.SupportedTemplates.FirstOrDefault(x => x.TemplateType == typeof(BlockPreviewController));
                    if (blockPreviewController != null)
                    {
                        args.SelectedTemplate = blockPreviewController;
                    }
                }
            }
        }
    }
}
