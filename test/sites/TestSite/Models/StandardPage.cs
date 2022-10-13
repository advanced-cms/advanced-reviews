using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace TestSite.Models
{
    [ContentType(GUID = "85FC1C77-0040-4D05-AF42-14C165D882C7")]
    public class StandardPage : PageData
    {
        public virtual ContentArea ContentArea { get; set; }
    }
}
