using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestSite.CategoryEditorDescriptor;

namespace TestSite.Models
{
    [ContentType(GUID = "85FC1C77-0040-4D05-AF42-14C165D882C7")]
    public class StandardPage : PageData
    {
        public virtual string Content { get; set; }

        public virtual string Heading { get; set; }
    }
}
