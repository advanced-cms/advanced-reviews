using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Alloy.Sample.Models.Pages
{
    [SiteContentType(GUID = "111CF12F-1F01-4EA0-922F-0778314DDAF0")]
    public class TestContentBlock : BlockData
    {
        public virtual string Title { get; set; }
    }

    [SiteContentType(GUID = "222CF12F-1F01-4EA0-922F-0778314DDAF0")]
    public class TestContentPage : SitePageData
    {
        public virtual string Title { get; set; }

        [AllowedTypes(typeof(TestContentBlock))]
        public virtual ContentReference Reference { get; set; }
    }
}
