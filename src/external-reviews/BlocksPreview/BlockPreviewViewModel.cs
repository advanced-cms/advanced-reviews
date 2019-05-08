using System.Collections.Generic;
using EPiServer.Core;

namespace AdvancedExternalReviews.BlocksPreview
{
    public class BlockPreviewViewModel
    {
        public IContent PreviewContent { get; set; }

        public BlockPreviewViewModel()
        {
            Areas = new List<PreviewArea>();
        }

        public List<PreviewArea> Areas { get; }

        public class PreviewArea
        {
            public bool Supported { get; set; }
            public string AreaName { get; set; }
            public string AreaTag { get; set; }
            public ContentArea ContentArea { get; set; }
        }
    }
}
