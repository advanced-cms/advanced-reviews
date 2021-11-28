namespace Advanced.CMS.ExternalReviews.BlocksPreview
{
    public class BlocksTemplateCoordinator
    {
        //TODO NETCORE
        // public static void OnTemplateResolved(object sender, TemplateResolverEventArgs args)
        // {
        //     var options = ServiceLocator.Current.GetInstance<ExternalReviewOptions>();
        //     if (!options.IsEnabled)
        //     {
        //         return;
        //     }
        //
        //     if (args.ItemToRender is BlockData)
        //     {
        //         if (args.RequestedCategory == TemplateTypeCategories.MvcController ||
        //             args.RequestedCategory == TemplateTypeCategories.Page)
        //         {
        //             if (!ExternalReview.IsInExternalReviewContext)
        //             {
        //                 return;
        //             }
        //
        //             var blockPreviewController =
        //                 args.SupportedTemplates.FirstOrDefault(x => x.TemplateType == typeof(BlockPreviewController));
        //             if (blockPreviewController != null)
        //             {
        //                 args.SelectedTemplate = blockPreviewController;
        //             }
        //         }
        //     }
        // }
    }
}
