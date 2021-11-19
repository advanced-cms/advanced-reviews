namespace Advanced.CMS.ExternalReviews.BlocksPreview
{
    // [TemplateDescriptor(
    //     Inherited = true,
    //     TemplateTypeCategory = TemplateTypeCategories.MvcController,
    //     Tags = new[] {"CustomNotUsedTag"},
    //     AvailableWithoutTag = false)]
    // [VisitorGroupImpersonation]
    // public class BlockPreviewController : ActionControllerBase, IRenderTemplate<BlockData>
    // {
    //     private readonly IContentLoader _contentLoader;
    //     private readonly TemplateResolver _templateResolver;
    //     private readonly DisplayOptions _displayOptions;
    //
    //     public BlockPreviewController(IContentLoader contentLoader, TemplateResolver templateResolver,
    //         DisplayOptions displayOptions)
    //     {
    //         _contentLoader = contentLoader;
    //         _templateResolver = templateResolver;
    //         _displayOptions = displayOptions;
    //     }
    //
    //     public ActionResult Index(IContent currentContent)
    //     {
    //         var contentArea = new ContentArea();
    //         contentArea.Items.Add(new ContentAreaItem
    //         {
    //             ContentLink = currentContent.ContentLink
    //         });
    //
    //         var model = new BlockPreviewViewModel
    //         {
    //             PreviewContent = currentContent,
    //         };
    //         model.Areas.AddRange(PopulateContentAreas(currentContent));
    //
    //         const string url = "Views/BlockPreview/Index.cshtml";
    //         if (ModuleResourceResolver.Instance.TryResolvePath(typeof(BlockPreviewController).Assembly, url,
    //             out var resolvedPath))
    //         {
    //             return View(resolvedPath, model);
    //         }
    //
    //         return new HttpNotFoundResult("Content not found");
    //     }
    //
    //     private IEnumerable<BlockPreviewViewModel.PreviewArea> PopulateContentAreas(IContent currentContent)
    //     {
    //         var supportedDisplayOptions = _displayOptions
    //             .Select(x => new { Tag = x.Tag, Name = x.Name, Supported = SupportsTag(currentContent, x.Tag) })
    //             .ToList();
    //
    //         var result = new List<BlockPreviewViewModel.PreviewArea>();
    //
    //         if (supportedDisplayOptions.Any(x => x.Supported))
    //         {
    //             foreach (var displayOption in supportedDisplayOptions)
    //             {
    //                 var contentArea = new ContentArea();
    //                 contentArea.Items.Add(new ContentAreaItem
    //                 {
    //                     ContentLink = currentContent.ContentLink
    //                 });
    //                 var areaModel = new BlockPreviewViewModel.PreviewArea
    //                 {
    //                     Supported = displayOption.Supported,
    //                     AreaTag = displayOption.Tag,
    //                     AreaName = displayOption.Name,
    //                     ContentArea = contentArea
    //                 };
    //                 result.Add(areaModel);
    //             }
    //         }
    //
    //         return result;
    //     }
    //
    //     private bool SupportsTag(IContent content, string tag)
    //     {
    //         var templateModel = _templateResolver.Resolve(HttpContext,
    //             content.GetOriginalType(),
    //             content,
    //             TemplateTypeCategories.MvcPartial,
    //             tag);
    //
    //         return templateModel != null;
    //     }
    // }
}
