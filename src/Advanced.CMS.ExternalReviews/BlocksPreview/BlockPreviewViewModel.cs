using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Advanced.CMS.ExternalReviews.BlocksPreview;

public class BlockPreviewViewModel
{
    public IContent PreviewContent { get; set; }

    public BlockPreviewViewModel()
    {
        var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
        Areas = new List<PreviewArea>();
        NotFound = string.Format(localizationService.GetString("/preview/norendereratall"), PreviewContent.Name);
    }

    public List<PreviewArea> Areas { get; }

    public string NotFound { get; set; }

    public class PreviewArea
    {
        public bool Supported { get; set; }
        public string AreaName { get; set; }
        public string AreaTag { get; set; }
        public ContentArea ContentArea { get; set; }
    }
}
